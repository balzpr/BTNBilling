using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Database
    {
        private string _filePath = "data.json";

        public List<User> LoadUsers()
        {
            if(!File.Exists(_filePath))
            {
                return new List<User>();
            }

            var jsonData = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<User>>(jsonData) ?? new List<User>();
        }

        public void SaveUsers(List<User> user)
        {
            var jsonData = JsonConvert.SerializeObject(user, Formatting.Indented);
            File.WriteAllText(_filePath, jsonData);
        }

        public void AddUsers(string role, string username, string password)
        {
            var users = LoadUsers();

            if(users.Exists(u => u.Username==username))
            {
                Notify.Show("User already exists.", true);
                return;
            }

            var user = new User { Password = password, Role = role, Username = username };
            users.Add(user);

            SaveUsers(users);

            Notify.Show($"User {username} added.");
        }

        public bool Login(string username, string password)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Username == username &&  u.Password == password);

            if(user == null)
            {
                Notify.Show("Failed to login. Invalid username or password.", true);
                return false;
            }

            return true;
        }
    }
}
