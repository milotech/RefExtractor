using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RefExtractor.Data;
using RefExtractor.Html;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace RefExtractor.Processing
{
    public class MainProcess
    {
        private IRepository _repo;
        private int _parallelRequests;
        private CancellationTokenSource _cancel;
        private Task _workTask;
        private string[] _supportedTags;

        #region events
        public event Action<Page> PageProcessStarted;
        public event Action<Page> PageProcessed;
        public event Action ProcessStarted;
        public event Action ProcessCompleted;
        #endregion

        public MainProcess(IRepository repo, int parallelRequests)
        {
            _repo = repo;
            _parallelRequests = parallelRequests;
            _cancel = new CancellationTokenSource();
            _supportedTags = TagProcessorsFactory.GetSupportedTags();
        }

        public void Start()
        {
            if (_workTask != null)
                throw new InvalidOperationException("Process is already started!");

            _workTask = Task.Factory.StartNew(Work, _cancel.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            
            Log("Процесс запущен");

            if (ProcessStarted != null)
                ProcessStarted.BeginInvoke(null, null);
        }

        public void Stop()
        {
            _cancel.Cancel();

            try
            {
                _workTask.Wait();
            }
            catch(AggregateException ex)
            {
                ex.Flatten();
                foreach(var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is TaskCanceledException)
                    {
                        Log("Процесс был остановлен по запросу");

                        if (ProcessCompleted != null)
                            ProcessCompleted();
                    }
                    else
                    {
                        Log(innerEx.ToString());
                        throw;
                    }
                }
            }
        }

        private void Work()
        {
            int tasksCount = 0;

            foreach(var page in _repo.GetPages())
            {
                CheckCancel();

                Log("Началась обработка страницы " + page.Url);

                if (PageProcessStarted != null)
                    PageProcessStarted.BeginInvoke(page, null, null);

                // переменная для замыканий
                Page p = page;

                // увеличиваем счетчик запущенных потоков
                Interlocked.Increment(ref tasksCount);

                var task = HtmlRetriever.GetContent(p.Url, _cancel.Token)
                    // подцепим обработку полученного контента
                    .ContinueWith((t) =>
                    {
                        try
                        {
                            if (t.Status == TaskStatus.RanToCompletion)
                            {
                                Log(p.Url + " получен Html контент");

                                int result = ProcessHtml(t.Result, p);
                                
                                Log(p.Url + string.Format(": обнаружено {0} элементов", result));
                                
                                if (PageProcessed != null)
                                    PageProcessed.BeginInvoke(p, null, null);
                            }
                            else if (t.Status == TaskStatus.Faulted)
                            {
                                var ex = t.Exception.InnerExceptions.First();
                                Log(ex.ToString());

                                throw ex;
                            }
                        }
                        catch(Exception ex)
                        {                            
                            Log(p.Url + ": " + (ex.InnerException == null ? ex.Message : ex.InnerException.Message));
                        }
                        finally
                        {
                            Interlocked.Decrement(ref tasksCount);
                        }
                            
                    });

                // не стартуем новые задачи, пока счетчик не позволит
                while (tasksCount >= _parallelRequests)
                {
                    CheckCancel();
                    Thread.Sleep(500);
                }
            }

            Log("Ожидаем окончания работы потоков.");

            while(tasksCount != 0)
            {
                CheckCancel();
                Thread.Sleep(500);
            }

            Log("Процесс завершен");

            if (ProcessCompleted != null)
                ProcessCompleted.BeginInvoke(null, null);
        }

        private int ProcessHtml(string htmlContent, Page page)
        { 
            var parser = new HtmlTagsParser(htmlContent);
            int count = 0;
            HtmlTag tag = null;

            while((tag = parser.ReadNext(_supportedTags)) != null)
            {
                CheckCancel();
                
                var processor = TagProcessorsFactory.GetProcessor(tag.Name);
                
                // sync processing
                /*if (processor != null)
                    try
                    {
                        processor.Process(page, tag, _repo);
                    }
                    catch (Exception ex)
                    {
                        Log(string.Format("Обработка тега прошла неудачно: {0}\r\n    Тег {1}\r\n    Страница: {2}", ex.ToString(), tag.ToString(), page.Url));
                    }
                */

                //  async processing
                if (processor != null)
                {
                    var t = tag;
                    var p = page;
                    Task.Factory.StartNew(() => {
                        try
                        {
                            processor.Process(p, t, _repo);
                        }
                        catch(Exception ex)
                        {
                            Log(string.Format("Обработка тега прошла неудачно: {0}\r\n    Тег {1}\r\n    Страница: {2}", ex.ToString(), t.ToString(), p.Url));
                        }
                    }, _cancel.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
                }

                count++;
            }

            return count;
        }

        private void CheckCancel()
        {
            _cancel.Token.ThrowIfCancellationRequested();
        }

        private void Log(string message)
        {
            Trace.WriteLine(string.Format("{0} - {1}", DateTime.Now, message));
        }
    }
}
