using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;

public class DBTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string conn = "URI=file:" + Application.dataPath + "/database.s3db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();


        string sqlQuery = "SELECT value, id " + "FROM TestTable";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string value = reader.GetString(0);
            int id = reader.GetInt32(1);

            Debug.Log("ID = " + id + "  VALUE =" + value );
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
 
 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
