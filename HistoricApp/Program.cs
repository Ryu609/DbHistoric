using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
            var department2 = new Department(2, "II", 2);

            var entry3 = new Entry(3, 1, entry1.DateCreated,6550, DateTime.Now, 6551);
            var user3 = new User(3, 1, "Ayush", "Joynauth");
            var department3 = new Department(1, "IT", 3);

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
                var originalEntry = group.FirstOrDefault(i => i.Entry.DateUpdated == null).Entry;
                var originalUser = group.FirstOrDefault(i => i.User.UserId == originalEntry.HeaderId).User;
                var originalDepartment =
                    group.FirstOrDefault(i => i.Department.DepartmentId == originalEntry.HeaderId).Department;

                var EntryHistory = new HistoryViewModel()
                {
                    EntryId = originalEntry.EntryId,
                    DateCreated = originalEntry.DateCreated,
                    CreatedById = originalEntry.CreatedById
                };

                var modifications = new List<ModificationViewModel>();
                foreach (var myEntry in group.Select(s => s.Entry).OrderBy(s => s.HeaderId).Skip(1))
                {
                    
                    var myUser = group.FirstOrDefault(u => u.User.UserId == myEntry.HeaderId).User;
                    var myDepartment = group.FirstOrDefault(u => u.Department.DepartmentId == myEntry.HeaderId).Department;
                    var entryModifs = CompareProperties(originalEntry, myEntry);
                    var userModifs = CompareProperties(originalUser, myUser);
                    var departmentModifs = CompareProperties(originalDepartment, myDepartment);
                    modifications.AddRange(entryModifs.Select(i => new ModificationViewModel()
                    {
                        NewValue = i.NewValue,
                        OldValue = i.OldValue,
                        ColumnModified = i.PropertyName,
                        DateModified = myEntry.DateUpdated.Value,
                        ModifiedById = myEntry.UpdatedById.Value
                    }));
                    modifications.AddRange(userModifs.Select(i => new ModificationViewModel()
                    {
                        NewValue = i.NewValue,
                        OldValue = i.OldValue,
                        ColumnModified = i.PropertyName,
                        DateModified = myEntry.DateUpdated.Value,
                        ModifiedById = myEntry.UpdatedById.Value
                    }));
                    modifications.AddRange(departmentModifs.Select(i => new ModificationViewModel()
                    {
                        NewValue = i.NewValue,
                        OldValue = i.OldValue,
                        ColumnModified = i.PropertyName,
                        DateModified = myEntry.DateUpdated.Value,
                        ModifiedById = myEntry.UpdatedById.Value
                    }));

                    EntryHistory.Modifications = modifications;
                    originalEntry = myEntry;
                    originalDepartment = myDepartment;
                    originalUser = myUser;
                }

                Console.WriteLine(EntryHistory.EntryId);
                Console.WriteLine(EntryHistory.DateCreated);
                Console.WriteLine(EntryHistory.CreatedById);
                if(EntryHistory.Modifications == null)
                    Console.WriteLine("No Modification made to object");
                else
                {
                    foreach (var modifs in EntryHistory.Modifications)
                    {
                        Console.WriteLine(modifs.DateModified);
                        Console.WriteLine(modifs.ModifiedById);
                        Console.WriteLine(modifs.ColumnModified);
                        Console.WriteLine(modifs.OldValue);
                        Console.WriteLine(modifs.NewValue);
                    }
                }
            }
            Console.ReadLine();
        }

        public static List<Modifs> CompareProperties<T>(T original, T modified) where T : class
        {
            var modifs = new List<Modifs>();
            if (original != null && modified != null)
            {
                Type type = typeof (T);
                foreach (
                    System.Reflection.PropertyInfo pi in
                        type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {

                    object originalValue = type.GetProperty(pi.Name).GetValue(original,null);
                    object modifiedValue = type.GetProperty(pi.Name).GetValue(modified, null);

                    if (originalValue != modifiedValue || (originalValue == null ^ modifiedValue == null ))
                    {
                        modifs.Add( new Modifs()
                        {
                         PropertyName   = pi.Name,
                         OldValue = originalValue?.ToString(),
                         NewValue = modifiedValue?.ToString()

                        });
                    }

                }

            }
            return modifs;

        }
    }

    public class Modifs
    {
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

    }

    public class HistoryViewModel
    {
        public int EntryId { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedById { get; set; }
        public List<ModificationViewModel> Modifications { get; set; }

    }

    public class ModificationViewModel
    {
        public DateTime DateModified { get; set; }
        public int ModifiedById { get; set; }
        public string ColumnModified { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
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
