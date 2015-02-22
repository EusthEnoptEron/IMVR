using UnityEngine;
using System.Collections;
using System.Data.Sql;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Data.Linq.Mapping;

public class DBTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //string conn = "URI=file:" + Application.dataPath + "/database.s3db"; //Path to database.
        //var connection = System.Data.SQLite.Linq.SQLiteProviderFactory.Instance.CreateConnection();
        string _connection_string = "Data Source=" + Application.dataPath + "/database.s3db;DbLinqProvider=sqlite;DbLinqConnectionType=" + typeof(Mono.Data.Sqlite.SqliteConnection).AssemblyQualifiedName
 + ";";

        // in database method
        using (DataContext db = new DataContext(_connection_string))
        {
            var vals = (from values in db.GetTable<TestTable>()
                         select values);

            foreach (var val in vals)
            {
                Debug.Log(val.Value);
            }
        }
        //new SqliteConnection

        //IDbConnection dbconn;
        //dbconn = (IDbConnection)new SqliteConnection(conn);
        //dbconn.Open(); //Open connection to the database.
        //IDbCommand dbcmd = dbconn.CreateCommand();


        //string sqlQuery = "SELECT value, id " + "FROM TestTable";
        //dbcmd.CommandText = sqlQuery;
        //IDataReader reader = dbcmd.ExecuteReader();

        //while (reader.Read())
        //{
        //    string value = reader.GetString(0);
        //    int id = reader.GetInt32(1);

        //    Debug.Log("ID = " + id + "  VALUE =" + value );
        //}
         
        //reader.Close();
        //reader = null;
        //dbcmd.Dispose();
        //dbcmd = null;
        //dbconn.Close();
        //dbconn = null;
 
 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [Table(Name = "TestTable")]
    class TestTable
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name="id")]
        public int Id;
        
        [Column(Name="value")]
        public string Value;

    }
}
