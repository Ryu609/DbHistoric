using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoricApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var entry1 = new Entry(1, 1, DateTime.Now, 6550);
            var user1 = new User(1, 1, "Ayush", "Joynath");
            var department1 = new Department(1, "II", 1);

            var entry2  = new Entry(2, 2, DateTime.Now, 6550);
            var user2 = new User(2, 2, "Tanish", "Joynauth");
            var department2 = new Department(2, "II", 1);

            var entry3 = new Entry(3, 1, entry1.DateCreated,6550, DateTime.Now, 6551);
            var user3 = new User(1, 1, "Ayush", "Joynauth");
            var department3 = new Department(1, "IT", 1);

            var myExtract = new List<Extract>()
            {
                new Extract()
                {
                    Department = department1,
                    Entry = entry1,
                    User = user1

                },
                new Extract()
                {
                    Entry = entry2,
                    Department = department2,
                    User = user2
                },
                new Extract()
                {
                    Department = department3,
                    Entry = entry3,
                    User = user3
                }
            };

            var grouped = myExtract.GroupBy(i => i.Entry.EntryId);
            foreach (var group in grouped)
            {
                Console.WriteLine(group.Key);
                var createdDate = group.Select(g => g.Entry.DateCreated.ToString());
                foreach (var date in createdDate)
                {
                    Console.WriteLine(date);
                }
            }
            Console.ReadLine();
        }
    }

    public class Extract
    {
        
        public Entry Entry { get; set; }
        public User User { get; set; }
        public Department Department { get; set; }
    }


    public class User
    {
        public User(int userId, int entryId, string firstName, string lastname)
        {
            this.EntryId = entryId;
            this.UserId = userId;
            this.FirstName = firstName;
            this.LastName = lastname;

        }
        public int UserId { get; set; }
        public int EntryId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Department
    {
        public Department(int entryId, string departmentName, int departmentId)
        {
            this.EntryId = entryId;
            this.DepartmentId = departmentId;
            this.DepartmentName = departmentName;
        }
        public int EntryId { get; set; }
        public int DepartmentId { get; set;}
        public string DepartmentName { get; set; }
    }

    public class Entry
    {
        public Entry(int headerId, int entryId, DateTime createdDate, int createdbyId, DateTime? updatedDate = null, int? updatedbyId = null)
        {
            this.EntryId = entryId;
            this.CreatedById = createdbyId;
            this.DateUpdated = updatedDate;
            this.HeaderId = headerId;
            this.DateCreated = createdDate;
            this.UpdatedById = updatedbyId;

        }
        public int HeaderId { get; set; }
        public int EntryId { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedById { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? UpdatedById { get; set; }
    }




}
