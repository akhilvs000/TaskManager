using Realms;

namespace TaskManager.Model
{
    public class TodoDBItem : RealmObject
    {
		[PrimaryKey]
		public string Id { get; set; }

		public string Name { get; set; }

		public string Status { get; set; }

	}
}
