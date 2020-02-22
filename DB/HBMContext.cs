using System.Data.Entity;

namespace DB
{
    public class HBMContext : DbContext
    {
        static HBMContext()
        {
            Database.SetInitializer<HBMContext>(new ContextInitializer());
        }
        public HBMContext()
                : base("DefaultConnection")
        {

        }

        public DbSet<DeviceModel> Devices { get; set; }
        public DbSet<SignalModel> Signals { get; set; }
        public DbSet<ValuesModel> Values { get; set; }


    }
}
