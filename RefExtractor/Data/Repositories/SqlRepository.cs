using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefExtractor.Data.Repositories
{
    public class SqlRepository : IRepository
    {
        private string _connectionString;

        #region Sql scripts

        // имя таблиц захардкодил, ТЗ не перечит :)
        const string GetPagesScript = "SELECT [Id], [Url] FROM [page]";
        const string InsertLinkScript = "INSERT INTO [pageLink] ([PageId], [LinkUrl], [LinkType], [DateCollected]) VALUES(@PageId, @LinkUrl, @LinkType, @DT)";
        const string CreateLinksTableScript =
@"
IF NOT EXISTS (
SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[pageLink]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1
)
CREATE TABLE [dbo].[pageLink]
(
[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
[PageId] BIGINT NOT NULL, 
[LinkUrl] NVARCHAR(2000) NOT NULL, 
[LinkType] TINYINT NOT NULL, 
[DateCollected] DATETIME NOT NULL
)";

        #endregion

        public SqlRepository(string connectionString)
        {
            _connectionString = connectionString;

            CheckLinksTable();
        }

        public IEnumerable<Page> GetPages()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(GetPagesScript, connection))
                {
                    connection.Open();
                    using(var reader = command.ExecuteReader())
                        while (reader.Read())
                            yield return new Page { ID = reader.GetInt64(0), Url = reader.GetString(1) };
                }
        }

        public void AddReference(Reference reference)
        {
            DoAction(InsertLinkScript, command =>
            {
                command.Parameters.AddWithValue("@PageId", reference.PageID);
                command.Parameters.AddWithValue("@LinkUrl", reference.LinkUrl);
                command.Parameters.AddWithValue("@LinkType", reference.LinkType);
                command.Parameters.AddWithValue("@DT", reference.DateCollected);

                command.ExecuteNonQuery();
            });
        }

        private void CheckLinksTable()
        {
            DoAction(CreateLinksTableScript, command =>
            {
                command.ExecuteNonQuery();
            });
        }

        #region helpers

        private void DoAction(string script, Action<SqlCommand> action)
        {
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                using(SqlCommand command = new SqlCommand(script, connection))
                {
                    connection.Open();
                    action(command);
                }
            }
        }

        #endregion
    }
}
