using System.Data.Entity;

namespace DB
{
    class ContextInitializer: CreateDatabaseIfNotExists <HBMContext>
    {
        protected override void Seed(HBMContext db)
        {
            
        }
    }
}
