using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BlogProject.Models;
using MySql.Data.MySqlClient;

namespace BlogProject.Controllers
{
    public class AuthorDataController : ApiController
    {
        // The database context class which allows us to access our MySQL Database.
        private BlogDbContext Blog = new BlogDbContext();
        
        //This Controller Will access the authors table of our blog database.
        /// <summary>
        /// Returns a list of Authors in the system
        /// </summary>
        /// <example>GET api/AuthorData/ListAuthors</example>
        /// <returns>
        /// A list of authors (first names and last names)
        /// </returns>
        [HttpGet]
        [Route("api/AuthorData/ListAuthors/{SearchKey?}")]
        public IEnumerable<Author> ListAuthors(string SearchKey=null)
        {
            //Create an instance of a connection
            MySqlConnection Conn = Blog.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "Select * from Authors where lower(authorfname) like lower(@key) or lower(authorlname) like lower(@key) or lower(concat(authorfname, ' ', authorlname)) like lower(@key)";

            cmd.Parameters.AddWithValue("@key", "%" + SearchKey + "%");
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            //Create an empty list of Authors
            List<Author> Authors = new List<Author>{};

            //Loop Through Each Row the Result Set
            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                int AuthorId = Convert.ToInt32(ResultSet["authorid"]);
                string AuthorFname = ResultSet["authorfname"].ToString();
                string AuthorLname = ResultSet["authorlname"].ToString();
                string AuthorBio = ResultSet["authorbio"].ToString(); ;

                // This technique will work,
                //DateTime AuthorJoinDate = (DateTime)ResultSet["authorjoindate"];

                // This technique is safer!
                DateTime AuthorJoinDate;
                DateTime.TryParse(ResultSet["authorjoindate"].ToString(), out AuthorJoinDate);

                Author NewAuthor = new Author();
                NewAuthor.AuthorId = AuthorId;
                NewAuthor.AuthorFname = AuthorFname;
                NewAuthor.AuthorLname = AuthorLname;
                NewAuthor.AuthorBio = AuthorBio;

                //Add the Author Name to the List
                Authors.Add(NewAuthor);
            }

            //Close the connection between the MySQL Database and the WebServer
            Conn.Close();

            //Return the final list of author names
            return Authors;
        }



        /// <summary>
        /// Finds an author from the database
        /// </summary>
        /// <param name="id">The id to match against a primary key record in the MySQL Database</param>
        /// <returns>An author object</returns>
        [HttpGet]
        public Author FindAuthor(int id)
        {
            Author NewAuthor = new Author();

            //Create an instance of a connection
            MySqlConnection Conn = Blog.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "Select * from Authors where authorid = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                int AuthorId = (int)ResultSet["authorid"];
                string AuthorFname = (string)ResultSet["authorfname"];
                string AuthorLname = (string)ResultSet["authorlname"];
                string AuthorBio = (string)ResultSet["authorbio"];

                // This technique will work,
                //DateTime AuthorJoinDate = (DateTime)ResultSet["authorjoindate"];

                DateTime AuthorJoinDate;
                DateTime.TryParse(ResultSet["authorjoindate"].ToString(), out AuthorJoinDate);

                NewAuthor.AuthorId = AuthorId;
                NewAuthor.AuthorFname = AuthorFname;
                NewAuthor.AuthorLname = AuthorLname;
                NewAuthor.AuthorBio = AuthorBio;
                NewAuthor.AuthorJoinDate = AuthorJoinDate;
            }
            Conn.Close();

            return NewAuthor;
        }


        /// <summary>
        /// Removes an Author from the database
        /// </summary>
        /// <param name="id">The ID of the author to remove</param>
        /// <example>POST : /api/AuthorData/DeleteAuthor/3</example>
        /// <returns>Does not return anything.</returns>
        [HttpPost]
        public void DeleteAuthor(int id)
        {
            //Create an instance of a connection
            MySqlConnection Conn = Blog.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "Delete from authors where authorid=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();


        }

    }
}
