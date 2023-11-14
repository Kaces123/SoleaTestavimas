using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

using Org.Ktu.Isk.P175B602.Autonuoma.Models;
using Org.Ktu.Isk.P175B602.Autonuoma.ViewModels;


namespace Org.Ktu.Isk.P175B602.Autonuoma.Repositories
{
	public class UserRepo : IUserRepo
	{
		public List<User> List()
		{
			var users = new List<User>();

			string query = $@"SELECT * FROM `{Config.TblPrefix}users` ORDER BY id ASC";
			var dt = Sql.Query(query);

			foreach( DataRow item in dt )
			{
				users.Add(new User
				{
					Id = Convert.ToInt32(item["id"]),
					Name = Convert.ToString(item["name"]),
					Currency = Convert.ToInt32(item["currency"]),
					Email = Convert.ToString(item["email"]),
					Password = Convert.ToString(item["password"]),
				});
			}

			return users;
		}
		//This is used to find a user based on the id
		public User Find(int id)
		{
			var User = new User();

			var query = $@"SELECT * FROM `{Config.TblPrefix}users` WHERE id=?id";
			var dt = 
				Sql.Query(query, args => {
					args.Add("?id", MySqlDbType.Int32).Value = id;
				});

			foreach( DataRow item in dt )
			{
				User.Id = Convert.ToInt32(item["id"]);
				User.Name = Convert.ToString(item["name"]);
				User.Currency = Convert.ToInt32(item["currency"]);
				User.Email = Convert.ToString(item["email"]);
				User.Password = Convert.ToString(item["password"]);
			}

			return User;
		}
		//This is used to check when loggin in whether a user inputed a correct name and password

		public bool Exists(User user)
        {
			var users = new List<User>();
            // Implement the logic to check if the user already exists in your database.
            // This could involve querying your database to see if there is a user with the same name or email.
            // For demonstration purposes, assuming a simple in-memory list of users:
            return users.Any(u => u.Name == user.Name || u.Email == user.Email);
        }

		public void ChangePassword(User user, string newPassword)
   		 {
        // Implement logic to change the user's password
        // For example, you might update the user entity in the database
        user.Password = newPassword;
        // Your database update logic here...
   		 }
		public User Find(User user)
		{
			var User = new User();

			var query = $@"SELECT * FROM `{Config.TblPrefix}users` WHERE name=?name AND password=?password";
			var dt = 
				Sql.Query(query, args => {
					args.Add("?password", MySqlDbType.VarChar).Value = user.Password;
					args.Add("?name", MySqlDbType.VarChar).Value = user.Name;
				});
			foreach( DataRow item in dt )
			{
				User.Id = Convert.ToInt32(item["id"]);
				User.Name = Convert.ToString(item["name"]);
				User.Currency = Convert.ToInt32(item["currency"]);
				User.Email = Convert.ToString(item["email"]);
				User.Password = Convert.ToString(item["password"]);
			}

			return User;
		}
		//This is used to check when registering whether an account with the same name or email already exist
		public User Find(string user){
			var User = new User();
			var query = $@"SELECT * FROM `{Config.TblPrefix}users` WHERE email=?email";
			var dt = 
				Sql.Query(query, args => {
					args.Add("?email", MySqlDbType.VarChar).Value = user;
				});
			foreach( DataRow item in dt )
			{
				User.Id = Convert.ToInt32(item["id"]);
				User.Name = Convert.ToString(item["name"]);
				User.Currency = Convert.ToInt32(item["currency"]);
				User.Email = Convert.ToString(item["email"]);
				User.Password = Convert.ToString(item["password"]);
			}

			return User;
		}
		public User Find(string user, int n){
			var User = new User();
			var query = $@"SELECT * FROM `{Config.TblPrefix}users` WHERE name=?name";
			var dt = 
				Sql.Query(query, args => {
					args.Add("?name", MySqlDbType.VarChar).Value = user;
				});
			foreach( DataRow item in dt )
			{
				User.Id = Convert.ToInt32(item["id"]);
				User.Name = Convert.ToString(item["name"]);
				User.Currency = Convert.ToInt32(item["currency"]);
				User.Email = Convert.ToString(item["email"]);
				User.Password = Convert.ToString(item["password"]);
			}

			return User;
		}

		public void Update(User User)
		{			
			var query = 
				$@"UPDATE `{Config.TblPrefix}users` 
				SET 
					name=?name,
					currency=?currency,
					email=?email,
					password=?password
				WHERE 
					id=?id";

			Sql.Update(query, args => {
				args.Add("?name", MySqlDbType.VarChar).Value = User.Name;
				args.Add("?id", MySqlDbType.VarChar).Value = User.Id;
				args.Add("?currency", MySqlDbType.VarChar).Value = User.Currency;
				args.Add("?email", MySqlDbType.VarChar).Value = User.Email;
				args.Add("?password", MySqlDbType.VarChar).Value = User.Password;
			});							
		}

		public void Insert(User User)
		{			
				var query =
				$@"INSERT INTO `{Config.TblPrefix}users`
				(
					id,
                    name,
					currency,
					email,
					password
				)
				VALUES(
					?id,
					?name,
					?currency,
					?email,
					?password
				)";

			Sql.Insert(query, args => {
				args.Add("?id", MySqlDbType.VarChar).Value = User.Id;
				args.Add("?name", MySqlDbType.VarChar).Value = User.Name;
				args.Add("?currency", MySqlDbType.VarChar).Value = User.Currency;
				args.Add("?email", MySqlDbType.VarChar).Value = User.Email;
				args.Add("?password", MySqlDbType.VarChar).Value = User.Password;
			});
		}

		public void Delete(int id)
		{			
			var query = $@"DELETE FROM `{Config.TblPrefix}users` where id=?id";
			Sql.Delete(query, args => {
				args.Add("?id", MySqlDbType.Int32).Value = id;
			});			
		}
	}
}