using SQLite;
using People.Models;
using System.Threading.Tasks;

namespace People;

public class PersonRepository
{
    private readonly string _dbPath;
    private SQLiteAsyncConnection conn;

    public string StatusMessage { get; set; }

    public PersonRepository(string dbPath)
    {
        _dbPath = dbPath;
    }

    private async Task Init()
    {
        if (conn != null)
            return;

        conn = new SQLiteAsyncConnection(_dbPath);
        await conn.CreateTableAsync<PersonRB>();
    }

    public async Task AddNewPerson(string name)
    {
        int result = 0;
        try
        {
            await Init();

            if (string.IsNullOrEmpty(name))
                throw new Exception("Valid name required");

            result = await conn.InsertAsync(new PersonRB { Name = name });
            StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, name);
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to add {0}. Error: {1}", name, ex.Message);
        }
    }

    public async Task<List<PersonRB>> GetAllPeople()
    {
        try
        {
            await Init();
            return await conn.Table<PersonRB>().ToListAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }

        return new List<PersonRB>();
    }
}

