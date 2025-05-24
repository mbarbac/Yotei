namespace Yotei.ORM.Relational.FakeDB;

// ========================================================
public static partial class DB
{
    /// <summary>
    /// The authoritative list of elements.
    /// </summary>
    public static ImmutableList<EmployeeDTO> Employees
    {
        get
        {
            if (_Employees == null)
            {
                _Employees = _EmployeesShort.ToImmutableList();

                if (IsLongDatabase)
                    _Employees = _Employees.AddRange(_EmployeesLong);
            }
            return _Employees;
        }
    }

    static ImmutableList<EmployeeDTO> _Employees = null!;

    readonly static EmployeeDTO[] _EmployeesShort = new[]
    {
        new EmployeeDTO() { Id = "1001", Active = true, CountryId = "us", FirstName = "Tom", LastName = "Thomson", BirthDate = new DateOnly(1969, 9, 12), JoinDate = new DateOnly(2002, 1, 24), StartTime = new TimeOnly(10, 15, 0) }
        ,new EmployeeDTO() { Id = "1002", Active = true, ManagerId = "1001", CountryId = "us", FirstName = "Dave", LastName = "Alistair", BirthDate = new DateOnly(1959, 1, 23), JoinDate = new DateOnly(2001, 11, 9), StartTime = new TimeOnly(18, 0, 0) }
        ,new EmployeeDTO() { Id = "2001", Active = true, ManagerId = "1001", CountryId = "uk", FirstName = "Mohammed", LastName = "Al Kwahimin" }
        ,new EmployeeDTO() { Id = "2002", Active = true, ManagerId = "1001", CountryId = "uk", FirstName = "Andrew", LastName = "Mc Quanty", BirthDate = new DateOnly(1959, 8, 14), JoinDate = new DateOnly(2001, 11, 9), StartTime = new TimeOnly(18, 0, 0) }
        ,new EmployeeDTO() { Id = "2003", Active = true, ManagerId = "2001", CountryId = "es", FirstName = "David", LastName = "Perez de Manto" }
        ,new EmployeeDTO() { Id = "2005", Active = true, ManagerId = "2003", CountryId = "es", FirstName = "Juan", LastName = "Perez Gomez" }
        ,new EmployeeDTO() { Id = "2008", Active = true, ManagerId = "2003", CountryId = "es", FirstName = "Fernando", LastName = "Quesero Villaverde" }
        ,new EmployeeDTO() { Id = "2009", ManagerId = "2005", CountryId = "es", FirstName = "Antonio", LastName = "Martinez del Alamo" }
        ,new EmployeeDTO() { Id = "2006", Active = true, ManagerId = "2003", CountryId = "za", FirstName = "Richard", LastName = "Mc Donnel" }
        ,new EmployeeDTO() { Id = "2010", Active = true, ManagerId = "2006", CountryId = "za", FirstName = "Paul", LastName = "Brown" }
        ,new EmployeeDTO() { Id = "2011", Active = true, ManagerId = "2006", CountryId = "za", FirstName = "Nicole", LastName = "Weather" }
        ,new EmployeeDTO() { Id = "2004", ManagerId = "2003", CountryId = "ae", FirstName = "Hassan", LastName = "El Auly", BirthDate = new DateOnly(1972, 4, 25), JoinDate = new DateOnly(200, 11, 9), StartTime = new TimeOnly(12, 0, 0) }
        ,new EmployeeDTO() { Id = "2007", ManagerId = "2006", CountryId = "ae", FirstName = "John", LastName = "Burrogough" }
        ,new EmployeeDTO() { Id = "3001", ManagerId = "2011", CountryId = "jp", FirstName = "Asira", LastName = "Yamamoto", BirthDate = new DateOnly(1967, 2, 20), JoinDate = new DateOnly(2003, 8, 23), StartTime = new TimeOnly(5, 30, 0) }
        ,new EmployeeDTO() { Id = "3002", ManagerId = "2006", CountryId = "ca", FirstName = "Jonathan", LastName = "Willsney", BirthDate = new DateOnly(1972, 4, 7), JoinDate = new DateOnly(2002, 6, 1) }
    };

    readonly static EmployeeDTO[] _EmployeesLong ={
        new EmployeeDTO() { Id = "4000", Active = true, CountryId = "mx", ManagerId = "2005", FirstName = "Manuel", LastName = "Hamlin", BirthDate = new DateOnly(1972, 11, 4), JoinDate = new DateOnly(2008, 3, 23) }
        , new EmployeeDTO() { Id = "4001", Active = true, CountryId = "pt", ManagerId = "2005", FirstName = "Enrique", LastName = "Godoy" }
        , new EmployeeDTO() { Id = "4004", Active = true, CountryId = "pt", FirstName = "Dana", LastName = "Shanika", BirthDate = new DateOnly(1969, 10, 5) }
        , new EmployeeDTO() { Id = "4005", Active = true, CountryId = "mx", ManagerId = "1002", FirstName = "Elena", LastName = "Jover", BirthDate = new DateOnly(1974, 11, 2), JoinDate = new DateOnly(2005, 5, 18), StartTime = new TimeOnly(18, 35, 47) }
        , new EmployeeDTO() { Id = "4006", CountryId = "ae", ManagerId = "2001", FirstName = "Charvi", LastName = "Dismore", BirthDate = new DateOnly(1971, 5, 19) }
        /*20*/
        , new EmployeeDTO() { Id = "4007", CountryId = "es", ManagerId = "2011", FirstName = "Edric", LastName = "Newman", BirthDate = new DateOnly(1971, 8, 19), JoinDate = new DateOnly(2011, 9, 10) }
        , new EmployeeDTO() { Id = "4009", Active = true, CountryId = "ie", ManagerId = "1001", FirstName = "Amory", LastName = "Dipple", BirthDate = new DateOnly(1979, 5, 8), JoinDate = new DateOnly(2007, 3, 12) }
        , new EmployeeDTO() { Id = "4010", Active = true, CountryId = "ca", FirstName = "John", LastName = "Hammer", BirthDate = new DateOnly(1966, 3, 10) }
        , new EmployeeDTO() { Id = "4013", Active = true, CountryId = "us", FirstName = "John", LastName = "Baver", BirthDate = new DateOnly(1980, 8, 27) }
        , new EmployeeDTO() { Id = "4014", Active = true, CountryId = "usx", ManagerId = "2010", FirstName = "Chelsea", LastName = "Broomell", BirthDate = new DateOnly(1975, 7, 14) }
        /*25*/
        , new EmployeeDTO() { Id = "4016", Active = true, CountryId = "uk", ManagerId = "2008", FirstName = "Eugenio", LastName = "Oliva", BirthDate = new DateOnly(1964, 4, 22), JoinDate = new DateOnly(2016, 7, 2), StartTime = new TimeOnly(21, 22, 48) }
        , new EmployeeDTO() { Id = "4018", Active = true, CountryId = "us", ManagerId = "1001", FirstName = "Stuart", LastName = "Cornwell", BirthDate = new DateOnly(1971, 4, 25), JoinDate = new DateOnly(2007, 3, 24), StartTime = new TimeOnly(10, 12, 44) }
        , new EmployeeDTO() { Id = "4019", CountryId = "usx", ManagerId = "2010", FirstName = "Antonio", LastName = "Cullen", BirthDate = new DateOnly(1974, 7, 20), JoinDate = new DateOnly(2012, 5, 9), StartTime = new TimeOnly(22, 29, 11) }
        , new EmployeeDTO() { Id = "4020", Active = true, CountryId = "it", ManagerId = "1001", FirstName = "Thelma", LastName = "Keller", BirthDate = new DateOnly(1970, 3, 9), JoinDate = new DateOnly(2006, 1, 16), StartTime = new TimeOnly(3, 38, 6) }
        , new EmployeeDTO() { Id = "4021", Active = true, CountryId = "ie", ManagerId = "2011", FirstName = "Eduardo", LastName = "Horton" }
        /*30*/
        , new EmployeeDTO() { Id = "4023", Active = true, CountryId = "us", ManagerId = "2011", FirstName = "Prisha", LastName = "Batt", BirthDate = new DateOnly(1965, 2, 21), JoinDate = new DateOnly(2016, 8, 11), StartTime = new TimeOnly(6, 0, 34) }
        , new EmployeeDTO() { Id = "4025", Active = true, CountryId = "uk", ManagerId = "1001", FirstName = "Elias", LastName = "Astor", BirthDate = new DateOnly(1973, 11, 26) }
        , new EmployeeDTO() { Id = "4026", Active = true, CountryId = "mx", ManagerId = "2011", FirstName = "Enrique", LastName = "Shanika", BirthDate = new DateOnly(1984, 9, 25), JoinDate = new DateOnly(2009, 11, 9), StartTime = new TimeOnly(16, 12, 28) }
        , new EmployeeDTO() { Id = "4028", Active = true, CountryId = "es", ManagerId = "2001", FirstName = "Ernesto", LastName = "Baxendale", BirthDate = new DateOnly(1968, 8, 7), JoinDate = new DateOnly(2006, 3, 5), StartTime = new TimeOnly(1, 29, 3) }
        , new EmployeeDTO() { Id = "4029", CountryId = "ie", ManagerId = "1002", FirstName = "Dana", LastName = "Alarcon", BirthDate = new DateOnly(1980, 5, 14), JoinDate = new DateOnly(2011, 10, 26) }
        /*35*/
        , new EmployeeDTO() { Id = "4030", Active = true, CountryId = "za", ManagerId = "2011", FirstName = "Stuart", LastName = "Atkeson", BirthDate = new DateOnly(1976, 5, 6) }
        , new EmployeeDTO() { Id = "4031", Active = true, CountryId = "uk", ManagerId = "2008", FirstName = "Edric", LastName = "Peiro", BirthDate = new DateOnly(1984, 10, 3), JoinDate = new DateOnly(2014, 5, 24), StartTime = new TimeOnly(16, 18, 11) }
        , new EmployeeDTO() { Id = "4032", Active = true, CountryId = "ae", ManagerId = "1001", FirstName = "Stuart", LastName = "Oliva", BirthDate = new DateOnly(1969, 5, 10) }
        , new EmployeeDTO() { Id = "4033", CountryId = "pt", ManagerId = "1002", FirstName = "Ira", LastName = "Segarra", BirthDate = new DateOnly(1961, 4, 24), JoinDate = new DateOnly(2014, 11, 13) }
        , new EmployeeDTO() { Id = "4034", Active = true, CountryId = "za", ManagerId = "2003", FirstName = "Emilio", LastName = "Pulhart", BirthDate = new DateOnly(1980, 4, 18) }
        /*40*/
        , new EmployeeDTO() { Id = "4035", Active = true, CountryId = "ie", ManagerId = "2002", FirstName = "Pricilla", LastName = "Machado", BirthDate = new DateOnly(1959, 8, 17) }
        , new EmployeeDTO() { Id = "4036", Active = true, CountryId = "ie", ManagerId = "2005", FirstName = "Eshan", LastName = "Platten", BirthDate = new DateOnly(1973, 7, 12), JoinDate = new DateOnly(2015, 1, 13) }
        , new EmployeeDTO() { Id = "4037", Active = true, CountryId = "it", ManagerId = "2005", FirstName = "Sydney", LastName = "Parkinson", BirthDate = new DateOnly(1968, 4, 26) }
        , new EmployeeDTO() { Id = "4039", Active = true, CountryId = "za", ManagerId = "2010", FirstName = "Sahil", LastName = "Hamlin", BirthDate = new DateOnly(1970, 1, 20), JoinDate = new DateOnly(2008, 2, 5), StartTime = new TimeOnly(8, 49, 38) }
        , new EmployeeDTO() { Id = "4040", CountryId = "us", ManagerId = "2010", FirstName = "Edric", LastName = "Vyass" }
        /*45*/
        , new EmployeeDTO() { Id = "4041", Active = true, CountryId = "za", FirstName = "Samaira", LastName = "Batalla", BirthDate = new DateOnly(1982, 5, 5), JoinDate = new DateOnly(2015, 5, 19), StartTime = new TimeOnly(4, 35, 11) }
        , new EmployeeDTO() { Id = "4042", Active = true, CountryId = "usx", ManagerId = "2003", FirstName = "Samar", LastName = "Paxton", BirthDate = new DateOnly(1983, 4, 24) }
        , new EmployeeDTO() { Id = "4043", Active = true, CountryId = "pt", ManagerId = "2002", FirstName = "Samaira", LastName = "Edger", BirthDate = new DateOnly(1962, 5, 13), JoinDate = new DateOnly(2015, 7, 11), StartTime = new TimeOnly(5, 26, 10) }
        , new EmployeeDTO() { Id = "4044", Active = true, CountryId = "us", FirstName = "Beverly", LastName = "Varma", BirthDate = new DateOnly(1973, 2, 17), JoinDate = new DateOnly(2009, 2, 6), StartTime = new TimeOnly(20, 20, 56) }
        , new EmployeeDTO() { Id = "4045", Active = true, CountryId = "ie", ManagerId = "2010", FirstName = "Carter", LastName = "Yates", BirthDate = new DateOnly(1983, 4, 20) }
        /*50*/
        , new EmployeeDTO() { Id = "4046", Active = true, CountryId = "pt", ManagerId = "2001", FirstName = "Barbara", LastName = "Donie", BirthDate = new DateOnly(1982, 8, 15) }
        , new EmployeeDTO() { Id = "4047", Active = true, CountryId = "pt", ManagerId = "2003", FirstName = "Charvi", LastName = "Judge", BirthDate = new DateOnly(1972, 11, 24), JoinDate = new DateOnly(2016, 8, 15), StartTime = new TimeOnly(3, 44, 5) }
        , new EmployeeDTO() { Id = "4048", CountryId = "ae", ManagerId = "2005", FirstName = "Andrea", LastName = "Marrioter", BirthDate = new DateOnly(1975, 4, 9) }
        , new EmployeeDTO() { Id = "4050", Active = true, CountryId = "ie", ManagerId = "2011", FirstName = "Zara", LastName = "Battey", BirthDate = new DateOnly(1980, 4, 19), JoinDate = new DateOnly(2014, 8, 16), StartTime = new TimeOnly(10, 28, 7) }
        , new EmployeeDTO() { Id = "4051", Active = true, CountryId = "usx", ManagerId = "2002", FirstName = "Samar", LastName = "Hankin", BirthDate = new DateOnly(1984, 3, 5) }
        /*55*/
        , new EmployeeDTO() { Id = "4052", CountryId = "ae", ManagerId = "2011", FirstName = "Charvi", LastName = "Melden", BirthDate = new DateOnly(1963, 7, 11), JoinDate = new DateOnly(2008, 7, 24), StartTime = new TimeOnly(6, 10, 50) }
        , new EmployeeDTO() { Id = "4053", Active = true, CountryId = "es", FirstName = "Samar", LastName = "Wesley", BirthDate = new DateOnly(1962, 1, 17), JoinDate = new DateOnly(2016, 6, 18) }
        , new EmployeeDTO() { Id = "4055", Active = true, CountryId = "it", ManagerId = "1002", FirstName = "Eduardo", LastName = "Moke", BirthDate = new DateOnly(1984, 10, 20) }
        , new EmployeeDTO() { Id = "4057", Active = true, CountryId = "ae", ManagerId = "2008", FirstName = "Navya", LastName = "Lee", BirthDate = new DateOnly(1969, 10, 19), JoinDate = new DateOnly(2006, 1, 6), StartTime = new TimeOnly(18, 56, 8) }
        , new EmployeeDTO() { Id = "4059", Active = true, CountryId = "mx", ManagerId = "1002", FirstName = "Ainsley", LastName = "Cordner" }
        /*60*/
        , new EmployeeDTO() { Id = "4060", Active = true, CountryId = "ae", ManagerId = "2003", FirstName = "Andrea", LastName = "Nordan", BirthDate = new DateOnly(1975, 3, 10), JoinDate = new DateOnly(2015, 4, 7), StartTime = new TimeOnly(16, 57, 50) }
        , new EmployeeDTO() { Id = "4062", Active = true, CountryId = "es", ManagerId = "2006", FirstName = "Blythe", LastName = "Batt", BirthDate = new DateOnly(1978, 11, 19), JoinDate = new DateOnly(2014, 11, 4) }
        , new EmployeeDTO() { Id = "4063", Active = true, CountryId = "pt", ManagerId = "2001", FirstName = "Edric", LastName = "Hammer", BirthDate = new DateOnly(1964, 1, 13), JoinDate = new DateOnly(2009, 7, 23) }
        , new EmployeeDTO() { Id = "4064", Active = true, CountryId = "za", ManagerId = "2003", FirstName = "Clara", LastName = "Pole", BirthDate = new DateOnly(1966, 10, 7) }
        , new EmployeeDTO() { Id = "4066", Active = true, CountryId = "usx", ManagerId = "2003", FirstName = "Ernesto", LastName = "Seton", BirthDate = new DateOnly(1962, 11, 27) }
        /*65*/
        , new EmployeeDTO() { Id = "4067", Active = true, CountryId = "za", ManagerId = "2005", FirstName = "Prisha", LastName = "Marriner", BirthDate = new DateOnly(1960, 8, 4), JoinDate = new DateOnly(2006, 1, 15), StartTime = new TimeOnly(9, 16, 31) }
        , new EmployeeDTO() { Id = "4069", Active = true, CountryId = "usx", ManagerId = "1002", FirstName = "Samar", LastName = "Wesley", BirthDate = new DateOnly(1974, 5, 2), JoinDate = new DateOnly(2006, 4, 10), StartTime = new TimeOnly(8, 29, 49) }
        , new EmployeeDTO() { Id = "4072", CountryId = "usx", ManagerId = "2001", FirstName = "Elvira", LastName = "Lawthomtom", BirthDate = new DateOnly(1977, 8, 12) }
        , new EmployeeDTO() { Id = "4073", CountryId = "it", ManagerId = "2011", FirstName = "Prynash", LastName = "Alcaraz", BirthDate = new DateOnly(1970, 6, 9), JoinDate = new DateOnly(2016, 1, 21), StartTime = new TimeOnly(21, 3, 46) }
        , new EmployeeDTO() { Id = "4077", Active = true, CountryId = "za", FirstName = "Elvira", LastName = "Batton", BirthDate = new DateOnly(1982, 8, 17), JoinDate = new DateOnly(2007, 11, 13), StartTime = new TimeOnly(13, 12, 45) }
        /*70*/
        , new EmployeeDTO() { Id = "4078", Active = true, CountryId = "za", ManagerId = "1002", FirstName = "Emilio", LastName = "Sinclair", BirthDate = new DateOnly(1962, 7, 24) }
        , new EmployeeDTO() { Id = "4080", Active = true, CountryId = "it", ManagerId = "2005", FirstName = "Thelma", LastName = "Fenning", BirthDate = new DateOnly(1974, 7, 13) }
        , new EmployeeDTO() { Id = "4081", Active = true, CountryId = "es", ManagerId = "2008", FirstName = "Balduino", LastName = "Prest", BirthDate = new DateOnly(1973, 6, 16), JoinDate = new DateOnly(2015, 8, 21) }
        , new EmployeeDTO() { Id = "4082", CountryId = "pt", ManagerId = "2006", FirstName = "Dana", LastName = "Taylor", BirthDate = new DateOnly(1979, 10, 24) }
        , new EmployeeDTO() { Id = "4083", Active = true, CountryId = "mx", ManagerId = "2005", FirstName = "Beverly", LastName = "Pinson", BirthDate = new DateOnly(1963, 4, 23) }
        /*75*/
        , new EmployeeDTO() { Id = "4084", Active = true, CountryId = "za", ManagerId = "2010", FirstName = "Elias", LastName = "Gadley" }
        , new EmployeeDTO() { Id = "4085", Active = true, CountryId = "usx", ManagerId = "1001", FirstName = "Emilio", LastName = "Chesnut", BirthDate = new DateOnly(1978, 1, 4) }
        , new EmployeeDTO() { Id = "4087", Active = true, CountryId = "usx", ManagerId = "1002", FirstName = "Bedford", LastName = "Fuertes", BirthDate = new DateOnly(1960, 11, 15), JoinDate = new DateOnly(2007, 10, 24), StartTime = new TimeOnly(17, 55, 35) }
        , new EmployeeDTO() { Id = "4089", Active = true, CountryId = "mx", ManagerId = "2006", FirstName = "Chelsea", LastName = "Parsley", BirthDate = new DateOnly(1973, 9, 15), JoinDate = new DateOnly(2006, 7, 1) }
        , new EmployeeDTO() { Id = "4090", Active = true, CountryId = "us", ManagerId = "2002", FirstName = "Balduino", LastName = "Godwin", BirthDate = new DateOnly(1983, 11, 22) }
        /*80*/
        , new EmployeeDTO() { Id = "4092", CountryId = "pt", ManagerId = "2001", FirstName = "Charvi", LastName = "Diaz", BirthDate = new DateOnly(1969, 8, 17) }
        , new EmployeeDTO() { Id = "4093", CountryId = "es", FirstName = "Rita", LastName = "Heiden", BirthDate = new DateOnly(1966, 3, 12) }
        , new EmployeeDTO() { Id = "4094", Active = true, CountryId = "it", ManagerId = "1002", FirstName = "Carter", LastName = "Simons", BirthDate = new DateOnly(1967, 8, 6) }
        , new EmployeeDTO() { Id = "4096", Active = true, CountryId = "us", ManagerId = "2006", FirstName = "Barbara", LastName = "Harley", BirthDate = new DateOnly(1979, 2, 27), JoinDate = new DateOnly(2015, 4, 21) }
        , new EmployeeDTO() { Id = "4098", CountryId = "za", ManagerId = "2005", FirstName = "Prynash", LastName = "Robbs", BirthDate = new DateOnly(1975, 11, 1) }
        /*85*/
        , new EmployeeDTO() { Id = "4099", Active = true, CountryId = "ca", ManagerId = "2005", FirstName = "John", LastName = "Paxton", BirthDate = new DateOnly(1975, 10, 17) }
        , new EmployeeDTO() { Id = "4100", Active = true, CountryId = "usx", ManagerId = "2003", FirstName = "Enrique", LastName = "Sayres", BirthDate = new DateOnly(1974, 8, 4) }
        , new EmployeeDTO() { Id = "4102", Active = true, CountryId = "za", ManagerId = "2006", FirstName = "Ernesto", LastName = "Trivedi", BirthDate = new DateOnly(1969, 3, 18) }
        , new EmployeeDTO() { Id = "4103", Active = true, CountryId = "za", ManagerId = "2008", FirstName = "Blythe", LastName = "Fleet", BirthDate = new DateOnly(1964, 8, 9), JoinDate = new DateOnly(2010, 7, 22) }
        , new EmployeeDTO() { Id = "4106", Active = true, CountryId = "pt", ManagerId = "2005", FirstName = "Sydney", LastName = "Sunshine", BirthDate = new DateOnly(1964, 5, 11), JoinDate = new DateOnly(2007, 5, 22), StartTime = new TimeOnly(8, 51, 47) }
        /*90*/
        , new EmployeeDTO() { Id = "4108", CountryId = "ie", ManagerId = "2003", FirstName = "Elisa", LastName = "Parkinson", BirthDate = new DateOnly(1959, 5, 21) }
        , new EmployeeDTO() { Id = "4110", Active = true, CountryId = "za", ManagerId = "2003", FirstName = "Ainsley", LastName = "Nobble", BirthDate = new DateOnly(1979, 8, 20), JoinDate = new DateOnly(2016, 6, 4), StartTime = new TimeOnly(21, 0, 14) }
        , new EmployeeDTO() { Id = "4111", Active = true, CountryId = "us", ManagerId = "1001", FirstName = "Coleman", LastName = "Leaner", BirthDate = new DateOnly(1964, 11, 18), JoinDate = new DateOnly(2015, 3, 20) }
        , new EmployeeDTO() { Id = "4112", CountryId = "us", ManagerId = "2010", FirstName = "Elias", LastName = "Groove", BirthDate = new DateOnly(1979, 3, 24), JoinDate = new DateOnly(2014, 6, 20) }
        , new EmployeeDTO() { Id = "4113", Active = true, CountryId = "za", FirstName = "Andrea", LastName = "Wesley", BirthDate = new DateOnly(1964, 9, 1), JoinDate = new DateOnly(2012, 9, 20) }
        /*95*/
        , new EmployeeDTO() { Id = "4114", Active = true, CountryId = "ae", ManagerId = "2003", FirstName = "Ainsley", LastName = "Godoy", BirthDate = new DateOnly(1965, 6, 5) }
        , new EmployeeDTO() { Id = "4115", Active = true, CountryId = "pt", FirstName = "Ira", LastName = "Kiana", BirthDate = new DateOnly(1984, 11, 13), JoinDate = new DateOnly(2010, 4, 6) }
        , new EmployeeDTO() { Id = "4117", Active = true, CountryId = "uk", ManagerId = "2002", FirstName = "John", LastName = "Radley" }
        , new EmployeeDTO() { Id = "4119", Active = true, CountryId = "ca", ManagerId = "2002", FirstName = "Diego", LastName = "Hoit", BirthDate = new DateOnly(1961, 6, 25) }
        , new EmployeeDTO() { Id = "4120", Active = true, CountryId = "ie", FirstName = "Prisha", LastName = "Nobble", BirthDate = new DateOnly(1982, 10, 3), JoinDate = new DateOnly(2005, 9, 19), StartTime = new TimeOnly(14, 18, 21) }
        /*100*/
        , new EmployeeDTO() { Id = "4122", Active = true, CountryId = "ae", ManagerId = "2001", FirstName = "Navya", LastName = "Batton", BirthDate = new DateOnly(1961, 2, 7) }
        , new EmployeeDTO() { Id = "4123", CountryId = "uk", ManagerId = "2010", FirstName = "Beverly", LastName = "Simons", BirthDate = new DateOnly(1976, 4, 17) }
        , new EmployeeDTO() { Id = "4127", Active = true, CountryId = "es", ManagerId = "2011", FirstName = "Sydney", LastName = "Peiro", BirthDate = new DateOnly(1977, 8, 17), JoinDate = new DateOnly(2007, 2, 22) }
        , new EmployeeDTO() { Id = "4131", Active = true, CountryId = "za", ManagerId = "2010", FirstName = "Prisha", LastName = "Risley", BirthDate = new DateOnly(1977, 1, 20) }
        , new EmployeeDTO() { Id = "4133", Active = true, CountryId = "pt", FirstName = "Carter", LastName = "Simons", BirthDate = new DateOnly(1972, 8, 20), JoinDate = new DateOnly(2016, 11, 23) }
        /*105*/
        , new EmployeeDTO() { Id = "4134", Active = true, CountryId = "za", ManagerId = "1002", FirstName = "Eshan", LastName = "Batterson", BirthDate = new DateOnly(1971, 2, 22), JoinDate = new DateOnly(2010, 2, 1) }
        , new EmployeeDTO() { Id = "4135", Active = true, CountryId = "mx", ManagerId = "1002", FirstName = "Pricilla", LastName = "Farham", BirthDate = new DateOnly(1968, 7, 5) }
        , new EmployeeDTO() { Id = "4136", Active = true, CountryId = "usx", ManagerId = "2003", FirstName = "Elvira", LastName = "Heavens" }
        , new EmployeeDTO() { Id = "4141", Active = true, CountryId = "usx", FirstName = "Elena", LastName = "Alder", BirthDate = new DateOnly(1965, 4, 3), JoinDate = new DateOnly(2006, 5, 27) }
        , new EmployeeDTO() { Id = "4142", Active = true, CountryId = "pt", ManagerId = "2006", FirstName = "Edison", LastName = "Fenning", BirthDate = new DateOnly(1977, 3, 24), JoinDate = new DateOnly(2013, 8, 24) }
        /*110*/
        , new EmployeeDTO() { Id = "4143", Active = true, CountryId = "usx", FirstName = "Rose", LastName = "Batt", BirthDate = new DateOnly(1975, 5, 9), JoinDate = new DateOnly(2007, 9, 8), StartTime = new TimeOnly(13, 21, 29) }
        , new EmployeeDTO() { Id = "4144", Active = true, CountryId = "it", ManagerId = "2008", FirstName = "Elisa", LastName = "Nobble", BirthDate = new DateOnly(1975, 6, 17), JoinDate = new DateOnly(2012, 11, 12) }
        , new EmployeeDTO() { Id = "4145", Active = true, CountryId = "ca", ManagerId = "2008", FirstName = "Rudolph", LastName = "Gomez", BirthDate = new DateOnly(1970, 8, 15), JoinDate = new DateOnly(2012, 8, 5), StartTime = new TimeOnly(8, 19, 40) }
        , new EmployeeDTO() { Id = "4146", Active = true, CountryId = "uk", ManagerId = "2006", FirstName = "Thelma", LastName = "Sundaram", BirthDate = new DateOnly(1979, 1, 11), JoinDate = new DateOnly(2008, 7, 13), StartTime = new TimeOnly(6, 26, 39) }
        , new EmployeeDTO() { Id = "4147", Active = true, CountryId = "mx", FirstName = "Beverly", LastName = "Redding", BirthDate = new DateOnly(1984, 4, 7) }
        /*115*/
        , new EmployeeDTO() { Id = "4148", Active = true, CountryId = "za", ManagerId = "2010", FirstName = "Clara", LastName = "Tiller", BirthDate = new DateOnly(1983, 10, 16), JoinDate = new DateOnly(2008, 4, 5) }
        , new EmployeeDTO() { Id = "4150", Active = true, CountryId = "pt", FirstName = "Eshan", LastName = "Baver", BirthDate = new DateOnly(1978, 3, 22), JoinDate = new DateOnly(2013, 6, 6), StartTime = new TimeOnly(20, 21, 35) }
        , new EmployeeDTO() { Id = "4151", Active = true, CountryId = "ca", ManagerId = "1001", FirstName = "Bedford", LastName = "Gadley" }
        , new EmployeeDTO() { Id = "4154", Active = true, CountryId = "usx", FirstName = "Blythe", LastName = "Batt", BirthDate = new DateOnly(1978, 4, 23) }
        , new EmployeeDTO() { Id = "4155", Active = true, CountryId = "ca", ManagerId = "2001", FirstName = "Elisa", LastName = "Teelford", BirthDate = new DateOnly(1968, 9, 9), JoinDate = new DateOnly(2015, 7, 18), StartTime = new TimeOnly(11, 39, 15) }
        /*120*/
        , new EmployeeDTO() { Id = "4156", CountryId = "us", ManagerId = "2005", FirstName = "Eugenio", LastName = "Worley", BirthDate = new DateOnly(1968, 10, 5) }
        , new EmployeeDTO() { Id = "4158", Active = true, CountryId = "pt", FirstName = "Samaira", LastName = "Parsley", BirthDate = new DateOnly(1978, 1, 5) }
        , new EmployeeDTO() { Id = "4159", Active = true, CountryId = "us", ManagerId = "2010", FirstName = "Barbara", LastName = "Uppal", BirthDate = new DateOnly(1961, 6, 24) }
        , new EmployeeDTO() { Id = "4160", Active = true, CountryId = "ie", FirstName = "Balduino", LastName = "Atcher" }
        , new EmployeeDTO() { Id = "4161", CountryId = "it", FirstName = "Pricilla", LastName = "Jones", BirthDate = new DateOnly(1983, 10, 3) }
        /*125*/
        , new EmployeeDTO() { Id = "4162", Active = true, CountryId = "es", ManagerId = "2008", FirstName = "Elias", LastName = "Utearra", BirthDate = new DateOnly(1963, 3, 16), JoinDate = new DateOnly(2015, 9, 21) }
        , new EmployeeDTO() { Id = "4163", Active = true, CountryId = "ca", ManagerId = "2011", FirstName = "Antonio", LastName = "Dismore", BirthDate = new DateOnly(1980, 9, 27), JoinDate = new DateOnly(2007, 8, 22) }
        , new EmployeeDTO() { Id = "4165", Active = true, CountryId = "ae", ManagerId = "1002", FirstName = "John", LastName = "Pulhart", BirthDate = new DateOnly(1962, 9, 10) }
        , new EmployeeDTO() { Id = "4167", Active = true, CountryId = "pt", ManagerId = "2005", FirstName = "Ainsley", LastName = "Suarez", BirthDate = new DateOnly(1977, 4, 27), JoinDate = new DateOnly(2013, 5, 2), StartTime = new TimeOnly(22, 31, 46) }
        , new EmployeeDTO() { Id = "4168", CountryId = "usx", ManagerId = "2003", FirstName = "Charvi", LastName = "Peiro", BirthDate = new DateOnly(1977, 7, 7), JoinDate = new DateOnly(2013, 10, 17), StartTime = new TimeOnly(14, 28, 14) }
        /*130*/
        , new EmployeeDTO() { Id = "4169", Active = true, CountryId = "uk", ManagerId = "2011", FirstName = "Dana", LastName = "Star", BirthDate = new DateOnly(1980, 1, 5), JoinDate = new DateOnly(2011, 8, 23) }
        , new EmployeeDTO() { Id = "4170", Active = true, CountryId = "ca", ManagerId = "2005", FirstName = "Eshan", LastName = "Sherwood", BirthDate = new DateOnly(1961, 7, 13) }
        , new EmployeeDTO() { Id = "4171", Active = true, CountryId = "ie", ManagerId = "2002", FirstName = "Ainsley", LastName = "Silva", BirthDate = new DateOnly(1984, 7, 11), JoinDate = new DateOnly(2016, 2, 26) }
        , new EmployeeDTO() { Id = "4172", CountryId = "ca", ManagerId = "1001", FirstName = "Prisha", LastName = "Camino", BirthDate = new DateOnly(1964, 10, 18), JoinDate = new DateOnly(2009, 4, 6), StartTime = new TimeOnly(13, 14, 56) }
        , new EmployeeDTO() { Id = "4173", Active = true, CountryId = "ie", ManagerId = "2003", FirstName = "Blythe", LastName = "Harley", BirthDate = new DateOnly(1978, 2, 7), JoinDate = new DateOnly(2013, 9, 2) }
        /*135*/
        , new EmployeeDTO() { Id = "4174", Active = true, CountryId = "es", ManagerId = "2003", FirstName = "Diego", LastName = "Baxley", BirthDate = new DateOnly(1965, 7, 6), JoinDate = new DateOnly(2008, 5, 24) }
        , new EmployeeDTO() { Id = "4176", Active = true, CountryId = "pt", ManagerId = "2005", FirstName = "Elvira", LastName = "Gadson", BirthDate = new DateOnly(1961, 7, 9), JoinDate = new DateOnly(2012, 4, 12) }
        , new EmployeeDTO() { Id = "4177", Active = true, CountryId = "it", ManagerId = "2003", FirstName = "Sydney", LastName = "Sunshine", BirthDate = new DateOnly(1961, 1, 1), JoinDate = new DateOnly(2010, 8, 11), StartTime = new TimeOnly(9, 58, 0) }
        , new EmployeeDTO() { Id = "4180", Active = true, CountryId = "ae", FirstName = "Wilbur", LastName = "Gomez", BirthDate = new DateOnly(1977, 11, 26) }
        , new EmployeeDTO() { Id = "4181", Active = true, CountryId = "ie", ManagerId = "2005", FirstName = "Elvira", LastName = "Radley", BirthDate = new DateOnly(1974, 4, 4) }
        /*140*/
        , new EmployeeDTO() { Id = "4182", Active = true, CountryId = "za", ManagerId = "1002", FirstName = "Brandon", LastName = "Horton", BirthDate = new DateOnly(1979, 8, 2), JoinDate = new DateOnly(2016, 5, 25), StartTime = new TimeOnly(17, 23, 6) }
        , new EmployeeDTO() { Id = "4183", Active = true, CountryId = "za", FirstName = "Sydney", LastName = "Atkeson", BirthDate = new DateOnly(1975, 7, 15), JoinDate = new DateOnly(2007, 2, 27) }
        , new EmployeeDTO() { Id = "4185", Active = true, CountryId = "pt", ManagerId = "2003", FirstName = "Clara", LastName = "Simons", BirthDate = new DateOnly(1967, 7, 1), JoinDate = new DateOnly(2016, 8, 6), StartTime = new TimeOnly(1, 23, 2) }
        , new EmployeeDTO() { Id = "4189", Active = true, CountryId = "us", ManagerId = "1002", FirstName = "Wilbur", LastName = "Diaz", BirthDate = new DateOnly(1968, 4, 17), JoinDate = new DateOnly(2014, 10, 11) }
        , new EmployeeDTO() { Id = "4190", Active = true, CountryId = "us", FirstName = "Balduino", LastName = "Cadwell", BirthDate = new DateOnly(1967, 10, 3) }
        /*145*/
        , new EmployeeDTO() { Id = "4191", Active = true, CountryId = "us", ManagerId = "2003", FirstName = "Ernesto", LastName = "Hilton", BirthDate = new DateOnly(1971, 10, 11), JoinDate = new DateOnly(2015, 8, 27), StartTime = new TimeOnly(4, 22, 22) }
        , new EmployeeDTO() { Id = "4192", CountryId = "usx", ManagerId = "2010", FirstName = "Edney", LastName = "Taplin", BirthDate = new DateOnly(1981, 2, 10) }
        , new EmployeeDTO() { Id = "4194", CountryId = "usx", ManagerId = "2002", FirstName = "Edney", LastName = "Batterson", BirthDate = new DateOnly(1969, 9, 8), JoinDate = new DateOnly(2007, 5, 25) }
        , new EmployeeDTO() { Id = "4195", Active = true, CountryId = "uk", ManagerId = "2003", FirstName = "Elena", LastName = "Hartwell", BirthDate = new DateOnly(1978, 4, 20), JoinDate = new DateOnly(2010, 2, 3), StartTime = new TimeOnly(9, 49, 12) }
        , new EmployeeDTO() { Id = "4196", CountryId = "it", FirstName = "David", LastName = "Esparza", BirthDate = new DateOnly(1969, 4, 16), JoinDate = new DateOnly(2008, 9, 19) }
        /*150*/
        , new EmployeeDTO() { Id = "4197", Active = true, CountryId = "mx", ManagerId = "2011", FirstName = "Mitul", LastName = "Zeandre", BirthDate = new DateOnly(1982, 9, 26) }
        , new EmployeeDTO() { Id = "4198", Active = true, CountryId = "pt", ManagerId = "2006", FirstName = "Elias", LastName = "Cornwell", BirthDate = new DateOnly(1977, 11, 9), JoinDate = new DateOnly(2009, 5, 13) }
        , new EmployeeDTO() { Id = "4199", Active = true, CountryId = "mx", ManagerId = "2010", FirstName = "Trenton", LastName = "Quiroga", BirthDate = new DateOnly(1966, 11, 13) }
        , new EmployeeDTO() { Id = "4200", Active = true, CountryId = "ie", ManagerId = "2010", FirstName = "Rose", LastName = "Brown" }
        , new EmployeeDTO() { Id = "4201", Active = true, CountryId = "za", ManagerId = "2011", FirstName = "Tina", LastName = "Varkey", BirthDate = new DateOnly(1966, 4, 24), JoinDate = new DateOnly(2010, 9, 17), StartTime = new TimeOnly(4, 52, 16) }
        /*155*/
        , new EmployeeDTO() { Id = "4202", CountryId = "mx", ManagerId = "2001", FirstName = "Eugenio", LastName = "Sunshine", BirthDate = new DateOnly(1963, 3, 15) }
        , new EmployeeDTO() { Id = "4203", Active = true, CountryId = "us", ManagerId = "2008", FirstName = "Manuel", LastName = "Wayman", BirthDate = new DateOnly(1978, 5, 21), JoinDate = new DateOnly(2016, 1, 12), StartTime = new TimeOnly(2, 8, 46) }
        , new EmployeeDTO() { Id = "4205", Active = true, CountryId = "usx", ManagerId = "2005", FirstName = "Zara", LastName = "Faulkner", BirthDate = new DateOnly(1961, 5, 4) }
        , new EmployeeDTO() { Id = "4207", Active = true, CountryId = "usx", ManagerId = "2002", FirstName = "Coleman", LastName = "Linn", BirthDate = new DateOnly(1970, 8, 7), JoinDate = new DateOnly(2006, 9, 5) }
        , new EmployeeDTO() { Id = "4209", Active = true, CountryId = "mx", ManagerId = "2001", FirstName = "Elena", LastName = "Mayoral", BirthDate = new DateOnly(1977, 3, 18), JoinDate = new DateOnly(2012, 10, 11), StartTime = new TimeOnly(7, 32, 31) }
        /*160*/
        , new EmployeeDTO() { Id = "4210", Active = true, CountryId = "ie", ManagerId = "2006", FirstName = "Enrique", LastName = "Pinckok", BirthDate = new DateOnly(1961, 4, 26), JoinDate = new DateOnly(2009, 4, 19) }
        , new EmployeeDTO() { Id = "4212", Active = true, CountryId = "uk", ManagerId = "1002", FirstName = "Beverly", LastName = "Germano", BirthDate = new DateOnly(1982, 5, 1), JoinDate = new DateOnly(2015, 11, 13) }
        , new EmployeeDTO() { Id = "4213", Active = true, CountryId = "ie", ManagerId = "1002", FirstName = "Elisa", LastName = "Lee", BirthDate = new DateOnly(1983, 4, 21), JoinDate = new DateOnly(2008, 8, 26), StartTime = new TimeOnly(18, 0, 36) }
        , new EmployeeDTO() { Id = "4214", CountryId = "ie", ManagerId = "2005", FirstName = "Coleman", LastName = "Kenerly", BirthDate = new DateOnly(1971, 3, 8), JoinDate = new DateOnly(2011, 3, 8) }
        , new EmployeeDTO() { Id = "4217", Active = true, CountryId = "usx", ManagerId = "2001", FirstName = "Eshan", LastName = "Jax" }
        /*165*/
        , new EmployeeDTO() { Id = "4218", Active = true, CountryId = "usx", ManagerId = "1002", FirstName = "Wilbur", LastName = "Marrioter", BirthDate = new DateOnly(1961, 3, 27), JoinDate = new DateOnly(2006, 5, 9) }
        , new EmployeeDTO() { Id = "4219", Active = true, CountryId = "mx", ManagerId = "2006", FirstName = "Diego", LastName = "Teelford", BirthDate = new DateOnly(1972, 8, 27), JoinDate = new DateOnly(2015, 11, 19) }
        , new EmployeeDTO() { Id = "4222", Active = true, CountryId = "ca", ManagerId = "2003", FirstName = "Blythe", LastName = "Robbs", BirthDate = new DateOnly(1960, 9, 17) }
        , new EmployeeDTO() { Id = "4224", Active = true, CountryId = "uk", ManagerId = "2001", FirstName = "Antonio", LastName = "Kiana", BirthDate = new DateOnly(1965, 10, 17), JoinDate = new DateOnly(2016, 9, 19), StartTime = new TimeOnly(11, 18, 56) }
        , new EmployeeDTO() { Id = "4225", Active = true, CountryId = "pt", ManagerId = "2008", FirstName = "Chelsea", LastName = "Faulkner", BirthDate = new DateOnly(1976, 11, 10), JoinDate = new DateOnly(2015, 8, 10) }
        /*170*/
        , new EmployeeDTO() { Id = "4226", Active = true, CountryId = "us", ManagerId = "2001", FirstName = "Edney", LastName = "Patesim", BirthDate = new DateOnly(1979, 8, 9) }
        , new EmployeeDTO() { Id = "4227", Active = true, CountryId = "pt", ManagerId = "2003", FirstName = "Sahil", LastName = "Barranco", BirthDate = new DateOnly(1983, 8, 13), JoinDate = new DateOnly(2012, 7, 25) }
        , new EmployeeDTO() { Id = "4228", Active = true, CountryId = "ca", FirstName = "Barbara", LastName = "Groove", BirthDate = new DateOnly(1977, 5, 22), JoinDate = new DateOnly(2014, 8, 1) }
        , new EmployeeDTO() { Id = "4229", Active = true, CountryId = "it", FirstName = "Eugenio", LastName = "Redding", BirthDate = new DateOnly(1977, 1, 17) }
        , new EmployeeDTO() { Id = "4230", Active = true, CountryId = "za", FirstName = "Navya", LastName = "Alcaraz" }
        /*175*/
        , new EmployeeDTO() { Id = "4231", Active = true, CountryId = "uk", ManagerId = "2003", FirstName = "Brandon", LastName = "Quant" }
        , new EmployeeDTO() { Id = "4232", Active = true, CountryId = "pt", ManagerId = "1001", FirstName = "Andrea", LastName = "Gardner", BirthDate = new DateOnly(1961, 8, 13), JoinDate = new DateOnly(2012, 4, 12) }
        , new EmployeeDTO() { Id = "4233", CountryId = "ae", FirstName = "Charvi", LastName = "Richarson", BirthDate = new DateOnly(1960, 9, 27) }
        , new EmployeeDTO() { Id = "4234", Active = true, CountryId = "mx", ManagerId = "2008", FirstName = "Ainsley", LastName = "Robbs", BirthDate = new DateOnly(1968, 11, 27), JoinDate = new DateOnly(2009, 4, 23) }
        , new EmployeeDTO() { Id = "4236", Active = true, CountryId = "uk", ManagerId = "2011", FirstName = "Elron", LastName = "Barranco", BirthDate = new DateOnly(1975, 5, 26) }
        /*180*/
        , new EmployeeDTO() { Id = "4237", Active = true, CountryId = "za", ManagerId = "2010", FirstName = "Zara", LastName = "Fleet", BirthDate = new DateOnly(1960, 4, 18) }
        , new EmployeeDTO() { Id = "4238", Active = true, CountryId = "ie", FirstName = "Rudolph", LastName = "Beacham", BirthDate = new DateOnly(1969, 8, 13), JoinDate = new DateOnly(2007, 9, 12), StartTime = new TimeOnly(9, 52, 30) }
        , new EmployeeDTO() { Id = "4239", Active = true, CountryId = "pt", ManagerId = "2011", FirstName = "Stuart", LastName = "Tinsleay", BirthDate = new DateOnly(1966, 5, 22), JoinDate = new DateOnly(2006, 8, 22), StartTime = new TimeOnly(21, 30, 33) }
        , new EmployeeDTO() { Id = "4241", Active = true, CountryId = "es", ManagerId = "1002", FirstName = "Ernesto", LastName = "Cadwell" }
        , new EmployeeDTO() { Id = "4242", Active = true, CountryId = "usx", FirstName = "Pricilla", LastName = "Purrington", BirthDate = new DateOnly(1963, 10, 17), JoinDate = new DateOnly(2008, 4, 17) }
        /*185*/
        , new EmployeeDTO() { Id = "4243", Active = true, CountryId = "uk", ManagerId = "2010", FirstName = "Edney", LastName = "Cullen", BirthDate = new DateOnly(1979, 7, 14), JoinDate = new DateOnly(2007, 2, 18) }
        , new EmployeeDTO() { Id = "4245", Active = true, CountryId = "mx", FirstName = "Prynash", LastName = "Keller", BirthDate = new DateOnly(1979, 9, 19) }
        , new EmployeeDTO() { Id = "4246", Active = true, CountryId = "ie", FirstName = "Edison", LastName = "Taylor", BirthDate = new DateOnly(1965, 6, 7) }
        , new EmployeeDTO() { Id = "4247", CountryId = "usx", ManagerId = "1001", FirstName = "Enrique", LastName = "Simons", BirthDate = new DateOnly(1984, 1, 5), JoinDate = new DateOnly(2014, 1, 23) }
        , new EmployeeDTO() { Id = "4248", Active = true, CountryId = "ae", FirstName = "Tina", LastName = "Astor" }
        /*190*/
        , new EmployeeDTO() { Id = "4250", Active = true, CountryId = "ie", ManagerId = "1001", FirstName = "Preston", LastName = "Farham", BirthDate = new DateOnly(1971, 1, 14) }
        , new EmployeeDTO() { Id = "4251", Active = true, CountryId = "za", ManagerId = "2010", FirstName = "Brandon", LastName = "Platten", BirthDate = new DateOnly(1964, 2, 20) }
        , new EmployeeDTO() { Id = "4252", Active = true, CountryId = "us", ManagerId = "2011", FirstName = "Barbara", LastName = "Meriwhether", BirthDate = new DateOnly(1983, 5, 26), JoinDate = new DateOnly(2015, 9, 26), StartTime = new TimeOnly(0, 35, 18) }
        , new EmployeeDTO() { Id = "4253", Active = true, CountryId = "usx", ManagerId = "1002", FirstName = "Andrea", LastName = "Pitts", BirthDate = new DateOnly(1966, 8, 3), JoinDate = new DateOnly(2016, 2, 10) }
        , new EmployeeDTO() { Id = "4255", CountryId = "ae", ManagerId = "2010", FirstName = "Clara", LastName = "Taplin", BirthDate = new DateOnly(1978, 2, 3) }
        /*195*/
        , new EmployeeDTO() { Id = "4256", CountryId = "es", FirstName = "Coleman", LastName = "Seaborn", BirthDate = new DateOnly(1965, 7, 18) }
        , new EmployeeDTO() { Id = "4258", Active = true, CountryId = "za", ManagerId = "2001", FirstName = "Amory", LastName = "Saxe", BirthDate = new DateOnly(1961, 1, 17), JoinDate = new DateOnly(2008, 7, 17) }
        , new EmployeeDTO() { Id = "4259", Active = true, CountryId = "es", FirstName = "Eugenio", LastName = "Hoit", BirthDate = new DateOnly(1969, 11, 22), JoinDate = new DateOnly(2010, 4, 4) }
        , new EmployeeDTO() { Id = "4260", Active = true, CountryId = "us", ManagerId = "2011", FirstName = "Wilbur", LastName = "Taylor", BirthDate = new DateOnly(1968, 7, 22), JoinDate = new DateOnly(2010, 5, 21) }
        , new EmployeeDTO() { Id = "4264", Active = true, CountryId = "es", ManagerId = "2001", FirstName = "Thelma", LastName = "Faulkner", BirthDate = new DateOnly(1962, 11, 16) }
        /*200*/
        , new EmployeeDTO() { Id = "4265", CountryId = "ie", ManagerId = "2010", FirstName = "Andrea", LastName = "Tiller", BirthDate = new DateOnly(1969, 3, 11), JoinDate = new DateOnly(2014, 8, 27), StartTime = new TimeOnly(9, 1, 10) }
        , new EmployeeDTO() { Id = "4268", Active = true, CountryId = "ca", FirstName = "Eugenio", LastName = "Towner", BirthDate = new DateOnly(1970, 1, 2), JoinDate = new DateOnly(2010, 9, 21) }
        , new EmployeeDTO() { Id = "4269", CountryId = "it", ManagerId = "2011", FirstName = "Prynash", LastName = "Dipple", BirthDate = new DateOnly(1969, 3, 21), JoinDate = new DateOnly(2014, 10, 12) }
        , new EmployeeDTO() { Id = "4270", Active = true, CountryId = "za", ManagerId = "2006", FirstName = "Cassandra", LastName = "Garland", BirthDate = new DateOnly(1976, 9, 10), JoinDate = new DateOnly(2008, 3, 6) }
        , new EmployeeDTO() { Id = "4271", Active = true, CountryId = "mx", ManagerId = "2008", FirstName = "Stuart", LastName = "Heiden", BirthDate = new DateOnly(1975, 7, 13) }
        /*205*/
        , new EmployeeDTO() { Id = "4272", Active = true, CountryId = "mx", ManagerId = "2001", FirstName = "Carter", LastName = "Kallen", BirthDate = new DateOnly(1984, 2, 2) }
        , new EmployeeDTO() { Id = "4273", Active = true, CountryId = "it", ManagerId = "2001", FirstName = "Antonio", LastName = "Jones" }
        , new EmployeeDTO() { Id = "4274", Active = true, CountryId = "ca", ManagerId = "2002", FirstName = "Blythe", LastName = "Beacham", BirthDate = new DateOnly(1983, 1, 20), JoinDate = new DateOnly(2012, 2, 20), StartTime = new TimeOnly(1, 32, 53) }
        , new EmployeeDTO() { Id = "4276", Active = true, CountryId = "it", ManagerId = "2005", FirstName = "Prynash", LastName = "Thompson", BirthDate = new DateOnly(1971, 7, 25) }
        , new EmployeeDTO() { Id = "4277", Active = true, CountryId = "ae", ManagerId = "2001", FirstName = "Samaira", LastName = "Beacham", BirthDate = new DateOnly(1967, 2, 1) }
        /*210*/
        , new EmployeeDTO() { Id = "4278", Active = true, CountryId = "pt", ManagerId = "2010", FirstName = "Jeremias", LastName = "Waylon", BirthDate = new DateOnly(1978, 9, 10) }
        , new EmployeeDTO() { Id = "4280", Active = true, CountryId = "us", ManagerId = "1001", FirstName = "Rita", LastName = "Tyreck", BirthDate = new DateOnly(1981, 11, 6), JoinDate = new DateOnly(2008, 10, 4), StartTime = new TimeOnly(11, 9, 4) }
        , new EmployeeDTO() { Id = "4281", CountryId = "ae", ManagerId = "2011", FirstName = "Pricilla", LastName = "Star", BirthDate = new DateOnly(1965, 11, 25) }
        , new EmployeeDTO() { Id = "4282", CountryId = "ie", ManagerId = "2003", FirstName = "Zara", LastName = "Hudson", BirthDate = new DateOnly(1974, 1, 4), JoinDate = new DateOnly(2007, 6, 13), StartTime = new TimeOnly(7, 22, 16) }
        , new EmployeeDTO() { Id = "4284", Active = true, CountryId = "ie", FirstName = "John", LastName = "Warden", BirthDate = new DateOnly(1960, 3, 15) }
        /*215*/
        , new EmployeeDTO() { Id = "4286", Active = true, CountryId = "es", FirstName = "Javier", LastName = "Pinckok", BirthDate = new DateOnly(1978, 8, 19), JoinDate = new DateOnly(2013, 3, 11), StartTime = new TimeOnly(19, 52, 41) }
        , new EmployeeDTO() { Id = "4287", Active = true, CountryId = "mx", FirstName = "Stuart", LastName = "Talford", BirthDate = new DateOnly(1975, 4, 14), JoinDate = new DateOnly(2009, 8, 11), StartTime = new TimeOnly(3, 40, 24) }
        , new EmployeeDTO() { Id = "4288", Active = true, CountryId = "es", FirstName = "Ainsley", LastName = "Dismore", BirthDate = new DateOnly(1980, 10, 11) }
        , new EmployeeDTO() { Id = "4290", Active = true, CountryId = "pt", ManagerId = "1001", FirstName = "Jeremias", LastName = "Alcaraz", BirthDate = new DateOnly(1978, 5, 4), JoinDate = new DateOnly(2005, 1, 24), StartTime = new TimeOnly(2, 10, 27) }
        , new EmployeeDTO() { Id = "4291", Active = true, CountryId = "usx", FirstName = "Manuel", LastName = "Lumbrearas", BirthDate = new DateOnly(1964, 9, 25) }
        /*220*/
        , new EmployeeDTO() { Id = "4292", CountryId = "ie", ManagerId = "2008", FirstName = "Brandon", LastName = "Melden", BirthDate = new DateOnly(1982, 2, 4), JoinDate = new DateOnly(2009, 6, 18), StartTime = new TimeOnly(12, 43, 13) }
        , new EmployeeDTO() { Id = "4293", Active = true, CountryId = "uk", ManagerId = "2008", FirstName = "Elisa", LastName = "Diaz" }
        , new EmployeeDTO() { Id = "4294", Active = true, CountryId = "us", ManagerId = "2003", FirstName = "Rita", LastName = "Baver", BirthDate = new DateOnly(1978, 8, 15), JoinDate = new DateOnly(2008, 7, 10) }
        , new EmployeeDTO() { Id = "4296", Active = true, CountryId = "ie", ManagerId = "2010", FirstName = "Jeremias", LastName = "Cordner", BirthDate = new DateOnly(1969, 3, 20), JoinDate = new DateOnly(2006, 4, 18), StartTime = new TimeOnly(20, 16, 12) }
        , new EmployeeDTO() { Id = "4302", Active = true, CountryId = "pt", ManagerId = "1001", FirstName = "Prisha", LastName = "Eastham", BirthDate = new DateOnly(1970, 8, 1), JoinDate = new DateOnly(2010, 9, 27) }
        /*225*/
        , new EmployeeDTO() { Id = "4303", CountryId = "us", FirstName = "Andrea", LastName = "Bayley", BirthDate = new DateOnly(1981, 9, 12), JoinDate = new DateOnly(2006, 4, 16), StartTime = new TimeOnly(12, 25, 12) }
        , new EmployeeDTO() { Id = "4305", Active = true, CountryId = "za", FirstName = "Ira", LastName = "Lopez", BirthDate = new DateOnly(1965, 3, 24), JoinDate = new DateOnly(2008, 5, 22) }
        , new EmployeeDTO() { Id = "4307", Active = true, CountryId = "pt", FirstName = "Mitul", LastName = "Simons", BirthDate = new DateOnly(1965, 2, 4) }
        , new EmployeeDTO() { Id = "4309", Active = true, CountryId = "ae", FirstName = "Wilfred", LastName = "Saxe", BirthDate = new DateOnly(1972, 6, 7), JoinDate = new DateOnly(2013, 10, 8) }
        , new EmployeeDTO() { Id = "4310", CountryId = "za", ManagerId = "2011", FirstName = "Prynash", LastName = "Wyatt", BirthDate = new DateOnly(1966, 3, 9), JoinDate = new DateOnly(2005, 2, 25), StartTime = new TimeOnly(7, 19, 47) }
        /*230*/
        , new EmployeeDTO() { Id = "4311", CountryId = "es", ManagerId = "1002", FirstName = "Diego", LastName = "Batterson", BirthDate = new DateOnly(1983, 7, 4), JoinDate = new DateOnly(2013, 5, 14) }
        , new EmployeeDTO() { Id = "4313", Active = true, CountryId = "ie", FirstName = "Blythe", LastName = "Cage" }
        , new EmployeeDTO() { Id = "4314", Active = true, CountryId = "za", FirstName = "Charvi", LastName = "Fabra", BirthDate = new DateOnly(1980, 3, 16), JoinDate = new DateOnly(2015, 8, 19), StartTime = new TimeOnly(16, 42, 2) }
        , new EmployeeDTO() { Id = "4315", Active = true, CountryId = "es", ManagerId = "2003", FirstName = "Elias", LastName = "Meriwhether", BirthDate = new DateOnly(1959, 6, 19), JoinDate = new DateOnly(2008, 10, 6) }
        , new EmployeeDTO() { Id = "4316", Active = true, CountryId = "it", ManagerId = "2002", FirstName = "Trenton", LastName = "Baxendale", BirthDate = new DateOnly(1984, 8, 4), JoinDate = new DateOnly(2006, 9, 21) }
        /*235*/
        , new EmployeeDTO() { Id = "4317", Active = true, CountryId = "it", ManagerId = "2003", FirstName = "Pricilla", LastName = "Brown", BirthDate = new DateOnly(1960, 3, 4), JoinDate = new DateOnly(2008, 6, 18), StartTime = new TimeOnly(11, 44, 43) }
        , new EmployeeDTO() { Id = "4319", Active = true, CountryId = "za", ManagerId = "2005", FirstName = "Ira", LastName = "Tinsleay", BirthDate = new DateOnly(1978, 11, 20) }
        , new EmployeeDTO() { Id = "4321", Active = true, CountryId = "uk", ManagerId = "2002", FirstName = "Elias", LastName = "Layton", BirthDate = new DateOnly(1964, 9, 14), JoinDate = new DateOnly(2013, 2, 13), StartTime = new TimeOnly(13, 1, 2) }
        , new EmployeeDTO() { Id = "4322", Active = true, CountryId = "us", ManagerId = "1001", FirstName = "Zara", LastName = "Risley", BirthDate = new DateOnly(1965, 5, 4), JoinDate = new DateOnly(2007, 1, 15), StartTime = new TimeOnly(1, 49, 5) }
        , new EmployeeDTO() { Id = "4323", Active = true, CountryId = "ie", ManagerId = "2006", FirstName = "Rose", LastName = "Meriwhether", BirthDate = new DateOnly(1965, 9, 15), JoinDate = new DateOnly(2013, 10, 9), StartTime = new TimeOnly(3, 22, 21) }
        /*240*/
        , new EmployeeDTO() { Id = "4325", Active = true, CountryId = "us", ManagerId = "1002", FirstName = "Eugenio", LastName = "Locke" }
        , new EmployeeDTO() { Id = "4326", Active = true, CountryId = "ie", ManagerId = "2008", FirstName = "John", LastName = "Hurley", BirthDate = new DateOnly(1973, 7, 16), JoinDate = new DateOnly(2008, 2, 15) }
        , new EmployeeDTO() { Id = "4327", Active = true, CountryId = "it", ManagerId = "2001", FirstName = "Ernesto", LastName = "Broomell", BirthDate = new DateOnly(1962, 3, 24), JoinDate = new DateOnly(2009, 5, 11), StartTime = new TimeOnly(7, 27, 27) }
        , new EmployeeDTO() { Id = "4328", Active = true, CountryId = "uk", ManagerId = "1001", FirstName = "Carter", LastName = "Astor", BirthDate = new DateOnly(1979, 10, 27), JoinDate = new DateOnly(2011, 5, 9) }
        , new EmployeeDTO() { Id = "4330", Active = true, CountryId = "ae", ManagerId = "2005", FirstName = "Eugenio", LastName = "Taylor", BirthDate = new DateOnly(1968, 3, 5), JoinDate = new DateOnly(2014, 10, 19), StartTime = new TimeOnly(14, 55, 35) }
        /*245*/
        , new EmployeeDTO() { Id = "4331", Active = true, CountryId = "pt", FirstName = "Carter", LastName = "Hartwell", BirthDate = new DateOnly(1966, 1, 26), JoinDate = new DateOnly(2016, 2, 3) }
        , new EmployeeDTO() { Id = "4332", Active = true, CountryId = "uk", FirstName = "Sydney", LastName = "Melden", BirthDate = new DateOnly(1968, 7, 11), JoinDate = new DateOnly(2006, 11, 8) }
        , new EmployeeDTO() { Id = "4334", Active = true, CountryId = "za", FirstName = "Enrique", LastName = "Redondo" }
        , new EmployeeDTO() { Id = "4335", Active = true, CountryId = "us", ManagerId = "2002", FirstName = "Carter", LastName = "Shanika", BirthDate = new DateOnly(1977, 4, 14) }
        , new EmployeeDTO() { Id = "4336", Active = true, CountryId = "mx", ManagerId = "2002", FirstName = "Mitul", LastName = "Fuster", BirthDate = new DateOnly(1968, 9, 5) }
        /*250*/
        , new EmployeeDTO() { Id = "4337", CountryId = "za", ManagerId = "2010", FirstName = "Elena", LastName = "Judge", BirthDate = new DateOnly(1975, 8, 24), JoinDate = new DateOnly(2009, 2, 13) }
        , new EmployeeDTO() { Id = "4338", CountryId = "us", ManagerId = "2001", FirstName = "Elvira", LastName = "Redding", BirthDate = new DateOnly(1975, 11, 2) }
        , new EmployeeDTO() { Id = "4339", Active = true, CountryId = "ie", ManagerId = "1001", FirstName = "Advikha", LastName = "Machado", BirthDate = new DateOnly(1980, 1, 6), JoinDate = new DateOnly(2012, 5, 19), StartTime = new TimeOnly(1, 50, 7) }
        , new EmployeeDTO() { Id = "4340", Active = true, CountryId = "usx", ManagerId = "2011", FirstName = "Edison", LastName = "Sayres", BirthDate = new DateOnly(1968, 6, 9), JoinDate = new DateOnly(2007, 3, 7) }
        , new EmployeeDTO() { Id = "4341", Active = true, CountryId = "uk", FirstName = "Sahil", LastName = "Hayward", BirthDate = new DateOnly(1973, 3, 19) }
        /*255*/
        , new EmployeeDTO() { Id = "4342", Active = true, CountryId = "es", ManagerId = "2011", FirstName = "Elron", LastName = "Shanika", BirthDate = new DateOnly(1975, 8, 14) }
        , new EmployeeDTO() { Id = "4343", CountryId = "ie", ManagerId = "1001", FirstName = "Antonio", LastName = "Talford", BirthDate = new DateOnly(1975, 10, 18), JoinDate = new DateOnly(2010, 3, 9), StartTime = new TimeOnly(21, 36, 29) }
        , new EmployeeDTO() { Id = "4345", Active = true, CountryId = "pt", ManagerId = "2002", FirstName = "Samaira", LastName = "Marriner", BirthDate = new DateOnly(1961, 2, 4), JoinDate = new DateOnly(2008, 6, 16), StartTime = new TimeOnly(18, 30, 13) }
        , new EmployeeDTO() { Id = "4348", Active = true, CountryId = "mx", ManagerId = "2002", FirstName = "Andrea", LastName = "Alarcon", BirthDate = new DateOnly(1977, 2, 1) }
        , new EmployeeDTO() { Id = "4349", CountryId = "ie", FirstName = "Edric", LastName = "Batt", BirthDate = new DateOnly(1970, 6, 22), JoinDate = new DateOnly(2008, 3, 20) }
        /*260*/
        , new EmployeeDTO() { Id = "4353", Active = true, CountryId = "za", ManagerId = "2003", FirstName = "Javier", LastName = "Teelford" }
        , new EmployeeDTO() { Id = "4354", Active = true, CountryId = "uk", ManagerId = "1001", FirstName = "Elias", LastName = "Shadow", BirthDate = new DateOnly(1963, 8, 20), JoinDate = new DateOnly(2013, 7, 15) }
        , new EmployeeDTO() { Id = "4355", CountryId = "za", ManagerId = "1002", FirstName = "Preston", LastName = "Kallen", BirthDate = new DateOnly(1974, 11, 20), JoinDate = new DateOnly(2012, 5, 15) }
        , new EmployeeDTO() { Id = "4356", Active = true, CountryId = "it", ManagerId = "2001", FirstName = "Elron", LastName = "Shadow" }
        , new EmployeeDTO() { Id = "4357", Active = true, CountryId = "us", ManagerId = "1002", FirstName = "Navya", LastName = "Bayley", BirthDate = new DateOnly(1966, 4, 15), JoinDate = new DateOnly(2010, 10, 1) }
        /*265*/
        , new EmployeeDTO() { Id = "4359", Active = true, CountryId = "za", ManagerId = "2011", FirstName = "Elvira", LastName = "Hurley", BirthDate = new DateOnly(1982, 10, 5), JoinDate = new DateOnly(2010, 11, 14), StartTime = new TimeOnly(10, 16, 12) }
        , new EmployeeDTO() { Id = "4360", Active = true, CountryId = "mx", ManagerId = "2003", FirstName = "Eduardo", LastName = "Talford", BirthDate = new DateOnly(1983, 11, 20) }
        , new EmployeeDTO() { Id = "4361", CountryId = "it", ManagerId = "2010", FirstName = "Edison", LastName = "Pulhart" }
        , new EmployeeDTO() { Id = "4362", Active = true, CountryId = "us", FirstName = "Balduino", LastName = "Cornwell", BirthDate = new DateOnly(1970, 9, 8), JoinDate = new DateOnly(2008, 5, 9) }
        , new EmployeeDTO() { Id = "4363", Active = true, CountryId = "za", ManagerId = "2010", FirstName = "Trenton", LastName = "Quiroga", BirthDate = new DateOnly(1978, 2, 6), JoinDate = new DateOnly(2008, 9, 15), StartTime = new TimeOnly(4, 3, 7) }
        /*270*/
        , new EmployeeDTO() { Id = "4366", Active = true, CountryId = "ie", FirstName = "Wilfred", LastName = "Batte" }
        , new EmployeeDTO() { Id = "4367", CountryId = "za", ManagerId = "1002", FirstName = "Clara", LastName = "Prest", BirthDate = new DateOnly(1972, 7, 11), JoinDate = new DateOnly(2013, 5, 6), StartTime = new TimeOnly(14, 12, 56) }
        , new EmployeeDTO() { Id = "4369", Active = true, CountryId = "ie", ManagerId = "2005", FirstName = "Edison", LastName = "Thompson", BirthDate = new DateOnly(1970, 2, 19) }
        , new EmployeeDTO() { Id = "4370", Active = true, CountryId = "it", ManagerId = "2005", FirstName = "Barbara", LastName = "Homes", BirthDate = new DateOnly(1974, 8, 7) }
        , new EmployeeDTO() { Id = "4371", Active = true, CountryId = "ae", ManagerId = "2011", FirstName = "Blythe", LastName = "Holdker", BirthDate = new DateOnly(1961, 8, 18), JoinDate = new DateOnly(2013, 11, 14) }
        /*275*/
        , new EmployeeDTO() { Id = "4372", Active = true, CountryId = "pt", FirstName = "Ira", LastName = "Baxendale", BirthDate = new DateOnly(1979, 5, 25) }
        , new EmployeeDTO() { Id = "4375", CountryId = "it", ManagerId = "1001", FirstName = "Chelsea", LastName = "Jones", BirthDate = new DateOnly(1960, 11, 4) }
        , new EmployeeDTO() { Id = "4376", Active = true, CountryId = "es", ManagerId = "2011", FirstName = "Clara", LastName = "Trivedi", BirthDate = new DateOnly(1972, 1, 24), JoinDate = new DateOnly(2005, 10, 3) }
        , new EmployeeDTO() { Id = "4377", Active = true, CountryId = "uk", ManagerId = "2006", FirstName = "Beverly", LastName = "Prest", BirthDate = new DateOnly(1970, 10, 14), JoinDate = new DateOnly(2012, 8, 15) }
        , new EmployeeDTO() { Id = "4378", Active = true, CountryId = "it", ManagerId = "2001", FirstName = "Ernesto", LastName = "Infante" }
        /*280*/
			
        , new EmployeeDTO() { Id = "4379", Active = true, CountryId = "ca", FirstName = "Rita", LastName = "Shadow", BirthDate = new DateOnly(1960, 3, 8) }
        , new EmployeeDTO() { Id = "4380", Active = true, CountryId = "mx", FirstName = "Manuel", LastName = "Farham", BirthDate = new DateOnly(1967, 4, 17) }
        , new EmployeeDTO() { Id = "4381", Active = true, CountryId = "it", ManagerId = "2011", FirstName = "Sahil", LastName = "Tinsleay", BirthDate = new DateOnly(1979, 11, 26), JoinDate = new DateOnly(2005, 1, 19) }
        , new EmployeeDTO() { Id = "4382", Active = true, CountryId = "ie", ManagerId = "2006", FirstName = "Clara", LastName = "Layton", BirthDate = new DateOnly(1980, 5, 3) }
        , new EmployeeDTO() { Id = "4383", Active = true, CountryId = "pt", ManagerId = "2002", FirstName = "Stuart", LastName = "Kenerly", BirthDate = new DateOnly(1966, 4, 13) }
        /*285*/
        , new EmployeeDTO() { Id = "4384", CountryId = "ie", FirstName = "Preston", LastName = "Talford", BirthDate = new DateOnly(1984, 8, 27), JoinDate = new DateOnly(2008, 5, 26) }
        , new EmployeeDTO() { Id = "4385", Active = true, CountryId = "es", ManagerId = "2008", FirstName = "Prynash", LastName = "Redman", BirthDate = new DateOnly(1962, 2, 7), JoinDate = new DateOnly(2012, 4, 24) }
        , new EmployeeDTO() { Id = "4386", Active = true, CountryId = "ae", FirstName = "Amory", LastName = "Fabra", BirthDate = new DateOnly(1976, 9, 18) }
        , new EmployeeDTO() { Id = "4387", CountryId = "za", ManagerId = "1002", FirstName = "Ira", LastName = "Fuster", BirthDate = new DateOnly(1976, 7, 18), JoinDate = new DateOnly(2007, 9, 19) }
        , new EmployeeDTO() { Id = "4388", Active = true, CountryId = "uk", ManagerId = "1001", FirstName = "Norton", LastName = "Quiroga", BirthDate = new DateOnly(1967, 1, 23), JoinDate = new DateOnly(2008, 10, 14) }
        /*290*/
        , new EmployeeDTO() { Id = "4390", Active = true, CountryId = "mx", ManagerId = "2008", FirstName = "Wilbur", LastName = "Gracie", BirthDate = new DateOnly(1973, 11, 4), JoinDate = new DateOnly(2013, 2, 23), StartTime = new TimeOnly(1, 14, 27) }
        , new EmployeeDTO() { Id = "4391", Active = true, CountryId = "it", FirstName = "Edric", LastName = "Radley", BirthDate = new DateOnly(1974, 5, 21), JoinDate = new DateOnly(2013, 6, 18), StartTime = new TimeOnly(9, 0, 53) }
        , new EmployeeDTO() { Id = "4392", Active = true, CountryId = "ae", ManagerId = "1001", FirstName = "Wilfred", LastName = "Beach", BirthDate = new DateOnly(1984, 9, 7), JoinDate = new DateOnly(2005, 2, 13), StartTime = new TimeOnly(22, 34, 26) }
        , new EmployeeDTO() { Id = "4393", Active = true, CountryId = "mx", ManagerId = "2001", FirstName = "Enrique", LastName = "Sayres", BirthDate = new DateOnly(1963, 6, 22) }
        , new EmployeeDTO() { Id = "4396", Active = true, CountryId = "ae", ManagerId = "2011", FirstName = "Pricilla", LastName = "Jover", BirthDate = new DateOnly(1967, 9, 5), JoinDate = new DateOnly(2006, 6, 1) }
        /*295*/
        , new EmployeeDTO() { Id = "4397", Active = true, CountryId = "it", FirstName = "Eugenio", LastName = "Kurtis", BirthDate = new DateOnly(1960, 7, 13), JoinDate = new DateOnly(2006, 10, 5), StartTime = new TimeOnly(21, 55, 47) }
        , new EmployeeDTO() { Id = "4399", Active = true, CountryId = "es", ManagerId = "1002", FirstName = "Elvira", LastName = "Yates" }
        , new EmployeeDTO() { Id = "4400", Active = true, CountryId = "ca", ManagerId = "2011", FirstName = "Jeremias", LastName = "Yates", BirthDate = new DateOnly(1967, 5, 12), JoinDate = new DateOnly(2007, 8, 7) }
        , new EmployeeDTO() { Id = "4401", CountryId = "za", ManagerId = "2001", FirstName = "Barbara", LastName = "Hackett", BirthDate = new DateOnly(1982, 11, 27) }
        , new EmployeeDTO() { Id = "4402", Active = true, CountryId = "mx", ManagerId = "2006", FirstName = "Ernesto", LastName = "Edger", BirthDate = new DateOnly(1966, 5, 22) }
        /*300*/
        , new EmployeeDTO() { Id = "4403", Active = true, CountryId = "ie", FirstName = "Enrique", LastName = "Ranger" }
        , new EmployeeDTO() { Id = "4405", CountryId = "ca", ManagerId = "2006", FirstName = "Prynash", LastName = "Prest", BirthDate = new DateOnly(1981, 2, 15), JoinDate = new DateOnly(2012, 9, 20) }
        , new EmployeeDTO() { Id = "4406", Active = true, CountryId = "ae", ManagerId = "2011", FirstName = "Elisa", LastName = "Lee", BirthDate = new DateOnly(1973, 1, 24), JoinDate = new DateOnly(2016, 9, 17) }
        , new EmployeeDTO() { Id = "4407", Active = true, CountryId = "mx", ManagerId = "2006", FirstName = "Bedford", LastName = "Waylon", BirthDate = new DateOnly(1984, 7, 19), JoinDate = new DateOnly(2011, 11, 20) }
        , new EmployeeDTO() { Id = "4408", Active = true, CountryId = "ae", ManagerId = "2006", FirstName = "Elena", LastName = "Hartwell", BirthDate = new DateOnly(1976, 2, 2), JoinDate = new DateOnly(2005, 1, 25), StartTime = new TimeOnly(21, 51, 57) }
        /*305*/
        , new EmployeeDTO() { Id = "4409", CountryId = "ae", ManagerId = "1001", FirstName = "Sydney", LastName = "Nordan", BirthDate = new DateOnly(1971, 2, 10) }
        , new EmployeeDTO() { Id = "4410", Active = true, CountryId = "ca", ManagerId = "2011", FirstName = "David", LastName = "Adams", BirthDate = new DateOnly(1979, 7, 12), JoinDate = new DateOnly(2007, 5, 25) }
        , new EmployeeDTO() { Id = "4411", Active = true, CountryId = "es", ManagerId = "1002", FirstName = "Diego", LastName = "Seton", BirthDate = new DateOnly(1980, 9, 7), JoinDate = new DateOnly(2015, 6, 22) }
        , new EmployeeDTO() { Id = "4412", Active = true, CountryId = "mx", ManagerId = "2003", FirstName = "Enrique", LastName = "Marrioter", BirthDate = new DateOnly(1977, 8, 26), JoinDate = new DateOnly(2008, 9, 14) }
        , new EmployeeDTO() { Id = "4413", Active = true, CountryId = "es", ManagerId = "2011", FirstName = "Javier", LastName = "Varma", BirthDate = new DateOnly(1962, 10, 8), JoinDate = new DateOnly(2009, 10, 3) }
        /*310*/
        , new EmployeeDTO() { Id = "4414", Active = true, CountryId = "mx", ManagerId = "1001", FirstName = "Manuel", LastName = "Lawthomtom", BirthDate = new DateOnly(1981, 5, 4) }
        , new EmployeeDTO() { Id = "4417", Active = true, CountryId = "es", ManagerId = "2001", FirstName = "Pricilla", LastName = "Fuertes", BirthDate = new DateOnly(1983, 2, 5), JoinDate = new DateOnly(2011, 1, 9) }
        , new EmployeeDTO() { Id = "4418", Active = true, CountryId = "uk", ManagerId = "1001", FirstName = "Edney", LastName = "Hamlin", BirthDate = new DateOnly(1981, 10, 1) }
        , new EmployeeDTO() { Id = "4419", Active = true, CountryId = "es", FirstName = "Antonio", LastName = "Utearra" }
        , new EmployeeDTO() { Id = "4420", Active = true, CountryId = "us", ManagerId = "2010", FirstName = "Ainsley", LastName = "Hayward", BirthDate = new DateOnly(1978, 3, 14), JoinDate = new DateOnly(2012, 4, 26), StartTime = new TimeOnly(12, 14, 45) }
        /*315*/
        , new EmployeeDTO() { Id = "4421", Active = true, CountryId = "ae", ManagerId = "2002", FirstName = "Elvira", LastName = "Venkatesh", BirthDate = new DateOnly(1974, 5, 16), JoinDate = new DateOnly(2009, 8, 10) }
        , new EmployeeDTO() { Id = "4423", CountryId = "za", ManagerId = "2002", FirstName = "Elias", LastName = "Taylor", BirthDate = new DateOnly(1967, 1, 2), JoinDate = new DateOnly(2006, 4, 21), StartTime = new TimeOnly(17, 26, 45) }
        , new EmployeeDTO() { Id = "4425", CountryId = "ie", ManagerId = "2002", FirstName = "Charvi", LastName = "Chinery", BirthDate = new DateOnly(1964, 10, 27), JoinDate = new DateOnly(2010, 5, 26) }
        , new EmployeeDTO() { Id = "4426", Active = true, CountryId = "mx", FirstName = "Elena", LastName = "Black", BirthDate = new DateOnly(1970, 1, 22) }
        , new EmployeeDTO() { Id = "4428", Active = true, CountryId = "ie", ManagerId = "1002", FirstName = "Elron", LastName = "Diaz", BirthDate = new DateOnly(1984, 9, 24), JoinDate = new DateOnly(2006, 3, 3) }
        /*320*/
        , new EmployeeDTO() { Id = "4429", CountryId = "ie", FirstName = "Javier", LastName = "Fawcett", BirthDate = new DateOnly(1960, 2, 11), JoinDate = new DateOnly(2007, 10, 8) }
        , new EmployeeDTO() { Id = "4430", Active = true, CountryId = "uk", ManagerId = "2002", FirstName = "Trenton", LastName = "Hayward", BirthDate = new DateOnly(1979, 9, 12) }
        , new EmployeeDTO() { Id = "4431", Active = true, CountryId = "za", ManagerId = "2005", FirstName = "Navya", LastName = "Dismore", BirthDate = new DateOnly(1983, 3, 3), JoinDate = new DateOnly(2009, 10, 12) }
        , new EmployeeDTO() { Id = "4432", Active = true, CountryId = "ie", FirstName = "Prynash", LastName = "Taylor", BirthDate = new DateOnly(1976, 8, 20) }
        , new EmployeeDTO() { Id = "4433", Active = true, CountryId = "es", ManagerId = "2005", FirstName = "Rose", LastName = "Dismore", BirthDate = new DateOnly(1970, 2, 22), JoinDate = new DateOnly(2007, 11, 13) }
        /*325*/
        , new EmployeeDTO() { Id = "4434", Active = true, CountryId = "us", ManagerId = "2011", FirstName = "Mitul", LastName = "Tyreck", BirthDate = new DateOnly(1966, 11, 2), JoinDate = new DateOnly(2009, 8, 19) }
        , new EmployeeDTO() { Id = "4435", Active = true, CountryId = "es", FirstName = "Samaira", LastName = "Chinery", BirthDate = new DateOnly(1983, 4, 14), JoinDate = new DateOnly(2009, 1, 23), StartTime = new TimeOnly(4, 39, 19) }
        , new EmployeeDTO() { Id = "4436", Active = true, CountryId = "mx", ManagerId = "2003", FirstName = "Elena", LastName = "Sundaram", BirthDate = new DateOnly(1963, 2, 13), JoinDate = new DateOnly(2006, 9, 20) }
        , new EmployeeDTO() { Id = "4438", CountryId = "es", FirstName = "Tobin", LastName = "Robbs", BirthDate = new DateOnly(1983, 10, 6), JoinDate = new DateOnly(2006, 1, 10) }
        , new EmployeeDTO() { Id = "4441", Active = true, CountryId = "ae", FirstName = "Antonio", LastName = "Tyreck", BirthDate = new DateOnly(1982, 5, 26), JoinDate = new DateOnly(2008, 11, 26) }
        /*330*/
        , new EmployeeDTO() { Id = "4442", Active = true, CountryId = "ae", ManagerId = "2001", FirstName = "Rudolph", LastName = "Sherwood", BirthDate = new DateOnly(1963, 8, 13), JoinDate = new DateOnly(2016, 5, 21), StartTime = new TimeOnly(19, 25, 46) }
        , new EmployeeDTO() { Id = "4445", Active = true, CountryId = "za", ManagerId = "1001", FirstName = "Andrea", LastName = "Judge", BirthDate = new DateOnly(1974, 4, 5) }
        , new EmployeeDTO() { Id = "4446", Active = true, CountryId = "usx", ManagerId = "2006", FirstName = "Barbara", LastName = "Quiroga", BirthDate = new DateOnly(1964, 6, 11) }
        , new EmployeeDTO() { Id = "4447", Active = true, CountryId = "ae", ManagerId = "2001", FirstName = "Norton", LastName = "Pritt", BirthDate = new DateOnly(1966, 6, 18), JoinDate = new DateOnly(2010, 10, 23) }
        , new EmployeeDTO() { Id = "4448", Active = true, CountryId = "ca", ManagerId = "2011", FirstName = "Sydney", LastName = "Gracie", BirthDate = new DateOnly(1963, 6, 10), JoinDate = new DateOnly(2015, 4, 23), StartTime = new TimeOnly(12, 53, 38) }
        /*335*/
        , new EmployeeDTO() { Id = "4449", Active = true, CountryId = "mx", ManagerId = "2011", FirstName = "Tobin", LastName = "Calloway" }
        , new EmployeeDTO() { Id = "4451", Active = true, CountryId = "ae", ManagerId = "2001", FirstName = "Sahil", LastName = "White", BirthDate = new DateOnly(1970, 10, 21) }
        , new EmployeeDTO() { Id = "4453", CountryId = "uk", ManagerId = "2001", FirstName = "Norton", LastName = "Lumbrearas" }
        , new EmployeeDTO() { Id = "4456", Active = true, CountryId = "es", FirstName = "Diego", LastName = "Duque" }
        , new EmployeeDTO() { Id = "4458", Active = true, CountryId = "es", ManagerId = "2011", FirstName = "Antonio", LastName = "Radley", BirthDate = new DateOnly(1961, 1, 25) }
        /*340*/
        , new EmployeeDTO() { Id = "4461", Active = true, CountryId = "it", ManagerId = "2006", FirstName = "Dana", LastName = "Homes", BirthDate = new DateOnly(1962, 9, 23), JoinDate = new DateOnly(2015, 2, 11) }
        , new EmployeeDTO() { Id = "4462", Active = true, CountryId = "za", ManagerId = "2008", FirstName = "Ainsley", LastName = "Milton", BirthDate = new DateOnly(1963, 6, 18) }
        , new EmployeeDTO() { Id = "4464", Active = true, CountryId = "za", FirstName = "Prynash", LastName = "Bravo", BirthDate = new DateOnly(1964, 8, 20) }
        , new EmployeeDTO() { Id = "4465", CountryId = "ae", ManagerId = "2011", FirstName = "Rose", LastName = "Shanika", BirthDate = new DateOnly(1966, 2, 5), JoinDate = new DateOnly(2007, 7, 19) }
        , new EmployeeDTO() { Id = "4468", CountryId = "ae", ManagerId = "2005", FirstName = "Cassandra", LastName = "Lee", BirthDate = new DateOnly(1979, 4, 26), JoinDate = new DateOnly(2015, 4, 7) }
        /*345*/
        , new EmployeeDTO() { Id = "4469", Active = true, CountryId = "uk", FirstName = "Advikha", LastName = "Fuster", BirthDate = new DateOnly(1964, 3, 1) }
        , new EmployeeDTO() { Id = "4470", Active = true, CountryId = "us", ManagerId = "2010", FirstName = "Andrea", LastName = "Batterson", BirthDate = new DateOnly(1959, 1, 14), JoinDate = new DateOnly(2010, 2, 17), StartTime = new TimeOnly(7, 30, 18) }
        , new EmployeeDTO() { Id = "4471", Active = true, CountryId = "ca", ManagerId = "2001", FirstName = "Elron", LastName = "Machado", BirthDate = new DateOnly(1981, 4, 10) }
        , new EmployeeDTO() { Id = "4472", Active = true, CountryId = "us", FirstName = "Chelsea", LastName = "Pitts", BirthDate = new DateOnly(1978, 11, 14), JoinDate = new DateOnly(2016, 7, 26) }
        , new EmployeeDTO() { Id = "4473", Active = true, CountryId = "pt", FirstName = "Advikha", LastName = "Heiden", BirthDate = new DateOnly(1972, 3, 13), JoinDate = new DateOnly(2009, 9, 26) }
        /*350*/
        , new EmployeeDTO() { Id = "4474", CountryId = "pt", ManagerId = "2005", FirstName = "Edney", LastName = "Battey", BirthDate = new DateOnly(1959, 1, 1), JoinDate = new DateOnly(2011, 3, 6), StartTime = new TimeOnly(6, 11, 36) }
        , new EmployeeDTO() { Id = "4475", Active = true, CountryId = "it", FirstName = "Brandon", LastName = "Baxley", BirthDate = new DateOnly(1968, 11, 27) }
        , new EmployeeDTO() { Id = "4477", Active = true, CountryId = "ca", ManagerId = "2002", FirstName = "Eugenio", LastName = "Graeme", BirthDate = new DateOnly(1960, 10, 6) }
        , new EmployeeDTO() { Id = "4480", Active = true, CountryId = "es", ManagerId = "2003", FirstName = "Beverly", LastName = "Tinsleay" }
        , new EmployeeDTO() { Id = "4482", CountryId = "mx", ManagerId = "2002", FirstName = "Diego", LastName = "Mayoral", BirthDate = new DateOnly(1978, 5, 16), JoinDate = new DateOnly(2006, 1, 27) }
        /*355*/
        , new EmployeeDTO() { Id = "4483", Active = true, CountryId = "za", ManagerId = "2001", FirstName = "Brandon", LastName = "Baver", BirthDate = new DateOnly(1984, 10, 12) }
        , new EmployeeDTO() { Id = "4484", CountryId = "za", ManagerId = "1002", FirstName = "Rose", LastName = "Melden", BirthDate = new DateOnly(1979, 9, 5) }
        , new EmployeeDTO() { Id = "4485", Active = true, CountryId = "us", ManagerId = "1001", FirstName = "Barbara", LastName = "Pole", BirthDate = new DateOnly(1972, 6, 24), JoinDate = new DateOnly(2015, 2, 7), StartTime = new TimeOnly(6, 14, 37) }
        , new EmployeeDTO() { Id = "4486", CountryId = "us", ManagerId = "2001", FirstName = "Louise", LastName = "Yates", BirthDate = new DateOnly(1961, 2, 20), JoinDate = new DateOnly(2007, 7, 1), StartTime = new TimeOnly(20, 43, 44) }
        , new EmployeeDTO() { Id = "4487", CountryId = "us", ManagerId = "2001", FirstName = "Diego", LastName = "Eastham", BirthDate = new DateOnly(1968, 1, 26), JoinDate = new DateOnly(2011, 6, 11), StartTime = new TimeOnly(22, 29, 53) }
        /*360*/
        , new EmployeeDTO() { Id = "4489", Active = true, CountryId = "uk", FirstName = "Norton", LastName = "Taplin", BirthDate = new DateOnly(1961, 1, 12), JoinDate = new DateOnly(2010, 9, 18) }
        , new EmployeeDTO() { Id = "4491", Active = true, CountryId = "usx", ManagerId = "1001", FirstName = "Elvira", LastName = "Bronson", BirthDate = new DateOnly(1973, 9, 16), JoinDate = new DateOnly(2013, 1, 19) }
        , new EmployeeDTO() { Id = "4492", CountryId = "ae", ManagerId = "2005", FirstName = "Advikha", LastName = "Egerton" }
        , new EmployeeDTO() { Id = "4493", CountryId = "ca", ManagerId = "2011", FirstName = "Edison", LastName = "Yates", BirthDate = new DateOnly(1978, 9, 17), JoinDate = new DateOnly(2016, 9, 26) }
        , new EmployeeDTO() { Id = "4496", Active = true, CountryId = "mx", ManagerId = "2008", FirstName = "Sydney", LastName = "Platten", BirthDate = new DateOnly(1977, 8, 17), JoinDate = new DateOnly(2015, 8, 20) }
        /*365*/
        , new EmployeeDTO() { Id = "4497", Active = true, CountryId = "usx", FirstName = "Antonio", LastName = "Batalla", BirthDate = new DateOnly(1983, 3, 3), JoinDate = new DateOnly(2013, 2, 25), StartTime = new TimeOnly(17, 57, 31) }
        , new EmployeeDTO() { Id = "4498", Active = true, CountryId = "ca", ManagerId = "1002", FirstName = "Brandon", LastName = "Suarez", BirthDate = new DateOnly(1983, 5, 25) }
        , new EmployeeDTO() { Id = "4500", Active = true, CountryId = "za", ManagerId = "2008", FirstName = "Rita", LastName = "Graeme", BirthDate = new DateOnly(1975, 5, 6) }
        , new EmployeeDTO() { Id = "4501", Active = true, CountryId = "it", ManagerId = "2008", FirstName = "Barbara", LastName = "Pulhart", BirthDate = new DateOnly(1972, 8, 9) }
        , new EmployeeDTO() { Id = "4504", Active = true, CountryId = "ie", FirstName = "Samar", LastName = "Godwin", BirthDate = new DateOnly(1975, 3, 18), JoinDate = new DateOnly(2009, 6, 9) }
        /*370*/
        , new EmployeeDTO() { Id = "4505", Active = true, CountryId = "mx", ManagerId = "2003", FirstName = "Zara", LastName = "Gomez", BirthDate = new DateOnly(1979, 11, 20), JoinDate = new DateOnly(2010, 6, 19), StartTime = new TimeOnly(9, 17, 14) }
        , new EmployeeDTO() { Id = "4506", CountryId = "es", ManagerId = "2008", FirstName = "Chelsea", LastName = "Radley", BirthDate = new DateOnly(1967, 3, 11) }
        , new EmployeeDTO() { Id = "4509", Active = true, CountryId = "pt", FirstName = "Ainsley", LastName = "Hurley", BirthDate = new DateOnly(1978, 4, 14), JoinDate = new DateOnly(2007, 8, 26) }
        , new EmployeeDTO() { Id = "4513", Active = true, CountryId = "za", ManagerId = "2010", FirstName = "Louise", LastName = "Baxley", BirthDate = new DateOnly(1981, 2, 15), JoinDate = new DateOnly(2015, 4, 9) }
        , new EmployeeDTO() { Id = "4514", Active = true, CountryId = "ie", ManagerId = "2011", FirstName = "Jeremias", LastName = "Checa", BirthDate = new DateOnly(1973, 2, 9), JoinDate = new DateOnly(2012, 3, 13), StartTime = new TimeOnly(4, 45, 40) }
        /*375*/
        , new EmployeeDTO() { Id = "4515", Active = true, CountryId = "it", FirstName = "Emilio", LastName = "Wesley", BirthDate = new DateOnly(1982, 3, 21) }
        , new EmployeeDTO() { Id = "4516", Active = true, CountryId = "ca", ManagerId = "1001", FirstName = "Eshan", LastName = "Dipple", BirthDate = new DateOnly(1962, 4, 8), JoinDate = new DateOnly(2011, 7, 4) }
        , new EmployeeDTO() { Id = "4517", Active = true, CountryId = "uk", ManagerId = "1002", FirstName = "Blythe", LastName = "Baver", BirthDate = new DateOnly(1963, 1, 25), JoinDate = new DateOnly(2008, 8, 5) }
        , new EmployeeDTO() { Id = "4518", Active = true, CountryId = "pt", ManagerId = "2005", FirstName = "Emilio", LastName = "Tinsleay", BirthDate = new DateOnly(1971, 5, 15), JoinDate = new DateOnly(2009, 4, 7), StartTime = new TimeOnly(1, 1, 43) }
        , new EmployeeDTO() { Id = "4519", Active = true, CountryId = "usx", ManagerId = "2002", FirstName = "Ira", LastName = "Mitchum", BirthDate = new DateOnly(1968, 5, 3), JoinDate = new DateOnly(2006, 5, 18), StartTime = new TimeOnly(0, 25, 0) }
        /*380*/
        , new EmployeeDTO() { Id = "4521", Active = true, CountryId = "ca", ManagerId = "2002", FirstName = "Rudolph", LastName = "Hoit", BirthDate = new DateOnly(1976, 3, 7) }
        , new EmployeeDTO() { Id = "4522", Active = true, CountryId = "za", ManagerId = "2008", FirstName = "Rita", LastName = "Alcaraz", BirthDate = new DateOnly(1970, 8, 1), JoinDate = new DateOnly(2006, 3, 27) }
        , new EmployeeDTO() { Id = "4523", Active = true, CountryId = "us", ManagerId = "1001", FirstName = "Edric", LastName = "Worley", BirthDate = new DateOnly(1959, 11, 24), JoinDate = new DateOnly(2007, 8, 7) }
        , new EmployeeDTO() { Id = "4524", Active = true, CountryId = "usx", ManagerId = "2006", FirstName = "Wilbur", LastName = "Quinton", BirthDate = new DateOnly(1979, 6, 24), JoinDate = new DateOnly(2016, 1, 14), StartTime = new TimeOnly(14, 28, 32) }
        , new EmployeeDTO() { Id = "4525", Active = true, CountryId = "za", ManagerId = "1001", FirstName = "Samaira", LastName = "Chandra", BirthDate = new DateOnly(1966, 3, 27) }
        /*385*/
        , new EmployeeDTO() { Id = "4526", Active = true, CountryId = "ie", ManagerId = "1001", FirstName = "Beverly", LastName = "Calloway", BirthDate = new DateOnly(1961, 3, 8) }
        , new EmployeeDTO() { Id = "4527", Active = true, CountryId = "za", ManagerId = "2006", FirstName = "Ernesto", LastName = "Cordner", BirthDate = new DateOnly(1979, 4, 9), JoinDate = new DateOnly(2006, 2, 13) }
        , new EmployeeDTO() { Id = "4528", Active = true, CountryId = "ca", FirstName = "Prisha", LastName = "Batalla" }
        , new EmployeeDTO() { Id = "4529", CountryId = "ae", ManagerId = "1002", FirstName = "Bedford", LastName = "Cuny", BirthDate = new DateOnly(1967, 6, 10) }
        , new EmployeeDTO() { Id = "4530", Active = true, CountryId = "it", ManagerId = "1002", FirstName = "Edison", LastName = "Hammer", BirthDate = new DateOnly(1965, 2, 25), JoinDate = new DateOnly(2007, 4, 6) }
        /*390*/
        , new EmployeeDTO() { Id = "4531", Active = true, CountryId = "pt", FirstName = "Samaira", LastName = "Kallen", BirthDate = new DateOnly(1968, 10, 16), JoinDate = new DateOnly(2012, 5, 27) }
        , new EmployeeDTO() { Id = "4532", CountryId = "ie", ManagerId = "1001", FirstName = "Edric", LastName = "Diaz", BirthDate = new DateOnly(1974, 5, 19), JoinDate = new DateOnly(2013, 5, 18) }
        , new EmployeeDTO() { Id = "4534", CountryId = "pt", FirstName = "Zara", LastName = "Mayoral", BirthDate = new DateOnly(1969, 3, 6) }
        , new EmployeeDTO() { Id = "4536", Active = true, CountryId = "uk", ManagerId = "2008", FirstName = "Cassandra", LastName = "Atkeson" }
        , new EmployeeDTO() { Id = "4538", Active = true, CountryId = "it", ManagerId = "2003", FirstName = "Tobin", LastName = "Fawcett", BirthDate = new DateOnly(1979, 6, 8), JoinDate = new DateOnly(2012, 2, 27) }
        /*395*/
        , new EmployeeDTO() { Id = "4539", Active = true, CountryId = "za", FirstName = "Eshan", LastName = "Patesim", BirthDate = new DateOnly(1967, 9, 6), JoinDate = new DateOnly(2013, 6, 6), StartTime = new TimeOnly(10, 38, 23) }
        , new EmployeeDTO() { Id = "4542", Active = true, CountryId = "uk", FirstName = "Beverly", LastName = "Varkey" }
        , new EmployeeDTO() { Id = "4543", Active = true, CountryId = "za", ManagerId = "2001", FirstName = "Elisa", LastName = "Uppal", BirthDate = new DateOnly(1973, 1, 5) }
        , new EmployeeDTO() { Id = "4545", Active = true, CountryId = "ca", FirstName = "Diego", LastName = "Parsley", BirthDate = new DateOnly(1970, 7, 21), JoinDate = new DateOnly(2007, 11, 18) }
        , new EmployeeDTO() { Id = "4546", Active = true, CountryId = "es", ManagerId = "2002", FirstName = "Edric", LastName = "Newman", BirthDate = new DateOnly(1967, 8, 15) }
        /*400*/
        , new EmployeeDTO() { Id = "4547", Active = true, CountryId = "us", ManagerId = "2002", FirstName = "Eduardo", LastName = "Pritt", BirthDate = new DateOnly(1975, 6, 3), JoinDate = new DateOnly(2006, 10, 6) }
        , new EmployeeDTO() { Id = "4548", Active = true, CountryId = "ca", FirstName = "Ainsley", LastName = "Zola", BirthDate = new DateOnly(1969, 11, 6), JoinDate = new DateOnly(2011, 8, 11), StartTime = new TimeOnly(7, 50, 19) }
        , new EmployeeDTO() { Id = "4549", CountryId = "pt", FirstName = "Barbara", LastName = "Tyreck", BirthDate = new DateOnly(1981, 3, 4), JoinDate = new DateOnly(2015, 2, 14), StartTime = new TimeOnly(2, 24, 48) }
        , new EmployeeDTO() { Id = "4550", Active = true, CountryId = "pt", FirstName = "Samar", LastName = "Predmore", BirthDate = new DateOnly(1963, 11, 2) }
        , new EmployeeDTO() { Id = "4551", Active = true, CountryId = "es", FirstName = "Sydney", LastName = "Adams", BirthDate = new DateOnly(1971, 6, 17) }
        /*405*/
        , new EmployeeDTO() { Id = "4553", Active = true, CountryId = "us", ManagerId = "2003", FirstName = "Tobin", LastName = "Bala", BirthDate = new DateOnly(1975, 6, 20), JoinDate = new DateOnly(2009, 7, 22), StartTime = new TimeOnly(0, 48, 30) }
        , new EmployeeDTO() { Id = "4554", Active = true, CountryId = "ae", FirstName = "Prisha", LastName = "Groove", BirthDate = new DateOnly(1968, 3, 6) }
        , new EmployeeDTO() { Id = "4555", Active = true, CountryId = "pt", FirstName = "Eduardo", LastName = "Donns", BirthDate = new DateOnly(1979, 8, 2) }
        , new EmployeeDTO() { Id = "4556", CountryId = "ie", FirstName = "Carter", LastName = "Fleet", BirthDate = new DateOnly(1979, 4, 9) }
        , new EmployeeDTO() { Id = "4559", Active = true, CountryId = "usx", ManagerId = "2001", FirstName = "Wilbur", LastName = "Sunshine", BirthDate = new DateOnly(1981, 7, 24) }
        /*410*/
        , new EmployeeDTO() { Id = "4560", Active = true, CountryId = "pt", ManagerId = "2003", FirstName = "Elron", LastName = "Brown", BirthDate = new DateOnly(1961, 6, 19) }
        , new EmployeeDTO() { Id = "4562", CountryId = "ie", ManagerId = "2002", FirstName = "Amory", LastName = "Dismore", BirthDate = new DateOnly(1984, 10, 18) }
        , new EmployeeDTO() { Id = "4563", CountryId = "za", ManagerId = "2002", FirstName = "Blythe", LastName = "Robbs", BirthDate = new DateOnly(1959, 2, 20) }
        , new EmployeeDTO() { Id = "4564", Active = true, CountryId = "ca", ManagerId = "2003", FirstName = "Prisha", LastName = "Villena", BirthDate = new DateOnly(1983, 8, 22), JoinDate = new DateOnly(2009, 4, 7), StartTime = new TimeOnly(4, 26, 24) }
        , new EmployeeDTO() { Id = "4565", CountryId = "usx", ManagerId = "2001", FirstName = "Eshan", LastName = "Leaner" }
        /*415*/
        , new EmployeeDTO() { Id = "4566", Active = true, CountryId = "ae", ManagerId = "2001", FirstName = "Prisha", LastName = "Kurtis", BirthDate = new DateOnly(1984, 6, 8), JoinDate = new DateOnly(2005, 2, 23), StartTime = new TimeOnly(13, 53, 18) }
        , new EmployeeDTO() { Id = "4567", CountryId = "it", ManagerId = "2003", FirstName = "Beverly", LastName = "Garland", BirthDate = new DateOnly(1970, 10, 23), JoinDate = new DateOnly(2009, 3, 12) }
        , new EmployeeDTO() { Id = "4568", Active = true, CountryId = "za", FirstName = "Brandon", LastName = "Fragfton" }
        , new EmployeeDTO() { Id = "4569", Active = true, CountryId = "usx", FirstName = "Balduino", LastName = "Parkinson", BirthDate = new DateOnly(1966, 8, 6) }
        , new EmployeeDTO() { Id = "4572", CountryId = "it", ManagerId = "1002", FirstName = "Rita", LastName = "Purrington", BirthDate = new DateOnly(1974, 3, 20) }
        /*420*/
        , new EmployeeDTO() { Id = "4573", Active = true, CountryId = "pt", ManagerId = "2006", FirstName = "Norton", LastName = "Silva", BirthDate = new DateOnly(1969, 10, 9) }
        , new EmployeeDTO() { Id = "4574", CountryId = "pt", ManagerId = "2011", FirstName = "Eshan", LastName = "Atkeson", BirthDate = new DateOnly(1963, 11, 3) }
        , new EmployeeDTO() { Id = "4578", Active = true, CountryId = "mx", ManagerId = "2008", FirstName = "Bedford", LastName = "Parkinson", BirthDate = new DateOnly(1959, 7, 1) }
        , new EmployeeDTO() { Id = "4580", CountryId = "ae", ManagerId = "2011", FirstName = "Samar", LastName = "Tyreck", BirthDate = new DateOnly(1970, 5, 2) }
        , new EmployeeDTO() { Id = "4581", CountryId = "ie", ManagerId = "2003", FirstName = "Tobin", LastName = "Lopez", BirthDate = new DateOnly(1971, 10, 18) }
        /*425*/
        , new EmployeeDTO() { Id = "4582", Active = true, CountryId = "usx", ManagerId = "2002", FirstName = "Norton", LastName = "Bravo", BirthDate = new DateOnly(1981, 7, 1), JoinDate = new DateOnly(2013, 10, 19), StartTime = new TimeOnly(22, 57, 13) }
        , new EmployeeDTO() { Id = "4584", CountryId = "us", ManagerId = "1001", FirstName = "Carter", LastName = "Lawes", BirthDate = new DateOnly(1968, 7, 19), JoinDate = new DateOnly(2005, 7, 3), StartTime = new TimeOnly(4, 15, 12) }
        , new EmployeeDTO() { Id = "4586", Active = true, CountryId = "it", ManagerId = "2003", FirstName = "Beverly", LastName = "Barranco", BirthDate = new DateOnly(1983, 6, 25), JoinDate = new DateOnly(2007, 1, 9), StartTime = new TimeOnly(10, 7, 33) }
        , new EmployeeDTO() { Id = "4587", Active = true, CountryId = "us", FirstName = "Pricilla", LastName = "Lumbrearas", BirthDate = new DateOnly(1979, 7, 3) }
        , new EmployeeDTO() { Id = "4588", Active = true, CountryId = "ca", ManagerId = "2005", FirstName = "Sydney", LastName = "Tyreck", BirthDate = new DateOnly(1960, 9, 23), JoinDate = new DateOnly(2006, 9, 21) }
        /*430*/
        , new EmployeeDTO() { Id = "4589", Active = true, CountryId = "usx", ManagerId = "2010", FirstName = "Eshan", LastName = "Mitchum", BirthDate = new DateOnly(1967, 3, 26), JoinDate = new DateOnly(2010, 4, 20) }
        , new EmployeeDTO() { Id = "4591", Active = true, CountryId = "ca", FirstName = "Edney", LastName = "Camino", BirthDate = new DateOnly(1972, 3, 4) }
        , new EmployeeDTO() { Id = "4594", Active = true, CountryId = "ae", ManagerId = "2003", FirstName = "Balduino", LastName = "Godoy", BirthDate = new DateOnly(1983, 3, 2) }
        , new EmployeeDTO() { Id = "4595", Active = true, CountryId = "usx", ManagerId = "2006", FirstName = "Prynash", LastName = "Brookes", BirthDate = new DateOnly(1971, 6, 23), JoinDate = new DateOnly(2013, 9, 4), StartTime = new TimeOnly(16, 5, 11) }
        , new EmployeeDTO() { Id = "4596", CountryId = "us", FirstName = "Edney", LastName = "Hewitt", BirthDate = new DateOnly(1977, 5, 24) }
        /*435*/
        , new EmployeeDTO() { Id = "4597", CountryId = "za", ManagerId = "2011", FirstName = "David", LastName = "Hernando", BirthDate = new DateOnly(1970, 9, 21), JoinDate = new DateOnly(2007, 8, 17) }
        , new EmployeeDTO() { Id = "4598", Active = true, CountryId = "usx", FirstName = "Balduino", LastName = "Bronson", BirthDate = new DateOnly(1975, 3, 8), JoinDate = new DateOnly(2005, 8, 16), StartTime = new TimeOnly(14, 55, 21) }
        , new EmployeeDTO() { Id = "4600", Active = true, CountryId = "za", ManagerId = "2005", FirstName = "Cassandra", LastName = "Suarez", BirthDate = new DateOnly(1980, 1, 5) }
        , new EmployeeDTO() { Id = "4603", Active = true, CountryId = "us", FirstName = "Samar", LastName = "Kimber", BirthDate = new DateOnly(1978, 1, 21) }
        , new EmployeeDTO() { Id = "4604", Active = true, CountryId = "mx", ManagerId = "2005", FirstName = "Wilfred", LastName = "Feimster", BirthDate = new DateOnly(1972, 2, 13) }
        /*440*/
        , new EmployeeDTO() { Id = "4605", CountryId = "us", ManagerId = "1002", FirstName = "Ernesto", LastName = "Beach", BirthDate = new DateOnly(1973, 10, 24) }
        , new EmployeeDTO() { Id = "4606", CountryId = "ae", FirstName = "Wilbur", LastName = "Bayley" }
        , new EmployeeDTO() { Id = "4607", Active = true, CountryId = "it", FirstName = "Rose", LastName = "Sunshine", BirthDate = new DateOnly(1976, 8, 18), JoinDate = new DateOnly(2009, 3, 27) }
        , new EmployeeDTO() { Id = "4608", Active = true, CountryId = "usx", ManagerId = "2010", FirstName = "Stuart", LastName = "Battey", BirthDate = new DateOnly(1963, 1, 14) }
        , new EmployeeDTO() { Id = "4610", CountryId = "it", FirstName = "Rita", LastName = "Teelford", BirthDate = new DateOnly(1978, 3, 9) }
        /*445*/
        , new EmployeeDTO() { Id = "4611", Active = true, CountryId = "ca", FirstName = "Samaira", LastName = "Vyass", BirthDate = new DateOnly(1963, 6, 20), JoinDate = new DateOnly(2012, 9, 12) }
        , new EmployeeDTO() { Id = "4612", Active = true, CountryId = "usx", FirstName = "Coleman", LastName = "Richarson", BirthDate = new DateOnly(1963, 1, 24), JoinDate = new DateOnly(2005, 10, 24) }
        , new EmployeeDTO() { Id = "4613", Active = true, CountryId = "uk", ManagerId = "1001", FirstName = "Wilbur", LastName = "Veenesh", BirthDate = new DateOnly(1965, 11, 8), JoinDate = new DateOnly(2011, 3, 20), StartTime = new TimeOnly(12, 21, 18) }
        , new EmployeeDTO() { Id = "4614", CountryId = "pt", ManagerId = "2006", FirstName = "Navya", LastName = "Aswell", BirthDate = new DateOnly(1968, 8, 25), JoinDate = new DateOnly(2013, 10, 11) }
        , new EmployeeDTO() { Id = "4616", CountryId = "za", ManagerId = "1002", FirstName = "Sydney", LastName = "Sundaram", BirthDate = new DateOnly(1962, 11, 27), JoinDate = new DateOnly(2015, 10, 19) }
        /*450*/
        , new EmployeeDTO() { Id = "4618", Active = true, CountryId = "uk", ManagerId = "1001", FirstName = "John", LastName = "Batterson", BirthDate = new DateOnly(1968, 4, 23) }
        , new EmployeeDTO() { Id = "4620", Active = true, CountryId = "mx", FirstName = "Navya", LastName = "Platten", BirthDate = new DateOnly(1961, 2, 5), JoinDate = new DateOnly(2012, 5, 2) }
        , new EmployeeDTO() { Id = "4622", Active = true, CountryId = "usx", ManagerId = "2006", FirstName = "Wilfred", LastName = "Fleet" }
        , new EmployeeDTO() { Id = "4623", Active = true, CountryId = "pt", FirstName = "Coleman", LastName = "Edger", BirthDate = new DateOnly(1983, 3, 15) }
        , new EmployeeDTO() { Id = "4624", Active = true, CountryId = "es", ManagerId = "2002", FirstName = "Elron", LastName = "Goudin", BirthDate = new DateOnly(1959, 7, 9), JoinDate = new DateOnly(2010, 6, 24), StartTime = new TimeOnly(18, 36, 7) }
        /*455*/
        , new EmployeeDTO() { Id = "4625", Active = true, CountryId = "es", ManagerId = "2005", FirstName = "Samar", LastName = "Vyass" }
        , new EmployeeDTO() { Id = "4627", Active = true, CountryId = "it", FirstName = "Pricilla", LastName = "Hoit", BirthDate = new DateOnly(1984, 9, 11), JoinDate = new DateOnly(2006, 11, 5), StartTime = new TimeOnly(6, 20, 15) }
        , new EmployeeDTO() { Id = "4628", Active = true, CountryId = "es", ManagerId = "2005", FirstName = "Beverly", LastName = "Lumbrearas", BirthDate = new DateOnly(1977, 1, 5) }
        , new EmployeeDTO() { Id = "4629", CountryId = "ca", ManagerId = "2011", FirstName = "Cassandra", LastName = "Varma", BirthDate = new DateOnly(1960, 8, 23) }
        , new EmployeeDTO() { Id = "4630", CountryId = "ca", ManagerId = "2006", FirstName = "Ira", LastName = "Ortega" }
        /*460*/
        , new EmployeeDTO() { Id = "4633", Active = true, CountryId = "usx", FirstName = "Dana", LastName = "Fuster", BirthDate = new DateOnly(1979, 10, 4), JoinDate = new DateOnly(2012, 1, 3) }
        , new EmployeeDTO() { Id = "4634", CountryId = "ae", ManagerId = "2002", FirstName = "Dana", LastName = "Cadwell", BirthDate = new DateOnly(1964, 1, 3) }
        , new EmployeeDTO() { Id = "4635", CountryId = "mx", FirstName = "Ernesto", LastName = "Bronson", BirthDate = new DateOnly(1977, 10, 26), JoinDate = new DateOnly(2005, 1, 17) }
        , new EmployeeDTO() { Id = "4636", CountryId = "es", ManagerId = "2001", FirstName = "Brandon", LastName = "Halwell", BirthDate = new DateOnly(1978, 2, 13), JoinDate = new DateOnly(2012, 8, 1) }
        , new EmployeeDTO() { Id = "4637", Active = true, CountryId = "ie", ManagerId = "2002", FirstName = "Enrique", LastName = "Nordan", BirthDate = new DateOnly(1959, 2, 20) }
        /*465*/
        , new EmployeeDTO() { Id = "4639", Active = true, CountryId = "ca", FirstName = "Manuel", LastName = "Trivedi", BirthDate = new DateOnly(1971, 9, 10), JoinDate = new DateOnly(2005, 2, 3), StartTime = new TimeOnly(3, 4, 10) }
        , new EmployeeDTO() { Id = "4640", Active = true, CountryId = "usx", ManagerId = "2008", FirstName = "Ernesto", LastName = "Duque", BirthDate = new DateOnly(1976, 2, 15), JoinDate = new DateOnly(2016, 4, 19) }
        , new EmployeeDTO() { Id = "4641", Active = true, CountryId = "pt", ManagerId = "2008", FirstName = "Tina", LastName = "Hurley", BirthDate = new DateOnly(1979, 10, 5), JoinDate = new DateOnly(2014, 7, 24), StartTime = new TimeOnly(12, 56, 2) }
        , new EmployeeDTO() { Id = "4642", Active = true, CountryId = "it", ManagerId = "2011", FirstName = "Advikha", LastName = "Pinson", BirthDate = new DateOnly(1983, 8, 12), JoinDate = new DateOnly(2009, 7, 18) }
        , new EmployeeDTO() { Id = "4643", Active = true, CountryId = "uk", ManagerId = "2006", FirstName = "Antonio", LastName = "Purrington", BirthDate = new DateOnly(1968, 11, 19) }
        /*470*/
        , new EmployeeDTO() { Id = "4645", Active = true, CountryId = "es", FirstName = "Pricilla", LastName = "Gadson", BirthDate = new DateOnly(1984, 5, 17), JoinDate = new DateOnly(2008, 6, 10) }
        , new EmployeeDTO() { Id = "4646", Active = true, CountryId = "it", ManagerId = "2001", FirstName = "Chelsea", LastName = "Suarez", BirthDate = new DateOnly(1969, 11, 18) }
        , new EmployeeDTO() { Id = "4647", Active = true, CountryId = "ca", ManagerId = "2003", FirstName = "Bedford", LastName = "Barranco", BirthDate = new DateOnly(1978, 1, 22), JoinDate = new DateOnly(2005, 4, 3) }
        , new EmployeeDTO() { Id = "4648", Active = true, CountryId = "it", ManagerId = "1002", FirstName = "Elias", LastName = "Dismore", BirthDate = new DateOnly(1972, 4, 9) }
        , new EmployeeDTO() { Id = "4650", Active = true, CountryId = "usx", FirstName = "Balduino", LastName = "Homes", BirthDate = new DateOnly(1983, 4, 21), JoinDate = new DateOnly(2011, 4, 19), StartTime = new TimeOnly(8, 57, 40) }
        /*475*/
        , new EmployeeDTO() { Id = "4652", Active = true, CountryId = "it", ManagerId = "2008", FirstName = "Mitul", LastName = "Graeme", BirthDate = new DateOnly(1965, 9, 27), JoinDate = new DateOnly(2009, 7, 6) }
        , new EmployeeDTO() { Id = "4655", Active = true, CountryId = "ca", FirstName = "David", LastName = "Batton", BirthDate = new DateOnly(1963, 5, 27), JoinDate = new DateOnly(2007, 2, 10) }
        , new EmployeeDTO() { Id = "4656", Active = true, CountryId = "mx", ManagerId = "1001", FirstName = "Ernesto", LastName = "Broomell", BirthDate = new DateOnly(1979, 7, 3) }
        , new EmployeeDTO() { Id = "4658", Active = true, CountryId = "ca", FirstName = "Elron", LastName = "Varkey", BirthDate = new DateOnly(1978, 9, 24), JoinDate = new DateOnly(2009, 4, 12) }
        , new EmployeeDTO() { Id = "4659", CountryId = "mx", FirstName = "Elron", LastName = "Sinyard", BirthDate = new DateOnly(1971, 9, 15), JoinDate = new DateOnly(2010, 6, 19) }
        /*480*/
        , new EmployeeDTO() { Id = "4660", Active = true, CountryId = "usx", FirstName = "Elias", LastName = "Donie", BirthDate = new DateOnly(1974, 7, 12), JoinDate = new DateOnly(2015, 5, 4) }
        , new EmployeeDTO() { Id = "4662", CountryId = "us", FirstName = "John", LastName = "Towner", BirthDate = new DateOnly(1959, 1, 15), JoinDate = new DateOnly(2008, 2, 9), StartTime = new TimeOnly(10, 48, 1) }
        , new EmployeeDTO() { Id = "4663", Active = true, CountryId = "es", ManagerId = "2002", FirstName = "Emilio", LastName = "Adams" }
        , new EmployeeDTO() { Id = "4664", Active = true, CountryId = "us", ManagerId = "2003", FirstName = "Rose", LastName = "Brookes", BirthDate = new DateOnly(1968, 7, 18), JoinDate = new DateOnly(2006, 8, 4) }
        , new EmployeeDTO() { Id = "4665", Active = true, CountryId = "pt", ManagerId = "2010", FirstName = "Tina", LastName = "Kiana" }
        /*485*/
        , new EmployeeDTO() { Id = "4666", Active = true, CountryId = "us", ManagerId = "2002", FirstName = "Wilbur", LastName = "Feimster", BirthDate = new DateOnly(1984, 5, 16) }
        , new EmployeeDTO() { Id = "4667", Active = true, CountryId = "ca", FirstName = "Stuart", LastName = "Ugarte", BirthDate = new DateOnly(1963, 7, 11), JoinDate = new DateOnly(2009, 11, 24), StartTime = new TimeOnly(6, 56, 28) }
        , new EmployeeDTO() { Id = "4668", Active = true, CountryId = "usx", FirstName = "Eduardo", LastName = "Batterson", BirthDate = new DateOnly(1971, 2, 7), JoinDate = new DateOnly(2006, 11, 11) }
        , new EmployeeDTO() { Id = "4669", CountryId = "ca", ManagerId = "1002", FirstName = "Advikha", LastName = "Cullen", BirthDate = new DateOnly(1962, 8, 25), JoinDate = new DateOnly(2015, 7, 19) }
        , new EmployeeDTO() { Id = "4671", Active = true, CountryId = "mx", ManagerId = "1002", FirstName = "Chelsea", LastName = "Hewitt", BirthDate = new DateOnly(1964, 1, 9) }
        /*490*/
        , new EmployeeDTO() { Id = "4672", Active = true, CountryId = "es", ManagerId = "2010", FirstName = "Jeremias", LastName = "Thompson" }
        , new EmployeeDTO() { Id = "4673", Active = true, CountryId = "mx", ManagerId = "2001", FirstName = "Andrea", LastName = "Talford" }
        , new EmployeeDTO() { Id = "4674", Active = true, CountryId = "ae", FirstName = "Zara", LastName = "Radley", BirthDate = new DateOnly(1963, 10, 23), JoinDate = new DateOnly(2012, 10, 8), StartTime = new TimeOnly(10, 30, 33) }
        , new EmployeeDTO() { Id = "4676", Active = true, CountryId = "ie", FirstName = "Edric", LastName = "Atchley", BirthDate = new DateOnly(1980, 6, 19), JoinDate = new DateOnly(2008, 8, 16), StartTime = new TimeOnly(18, 32, 38) }
        , new EmployeeDTO() { Id = "4679", Active = true, CountryId = "usx", ManagerId = "1001", FirstName = "Diego", LastName = "Beach", BirthDate = new DateOnly(1959, 9, 2), JoinDate = new DateOnly(2016, 5, 10) }
        /*495*/
        , new EmployeeDTO() { Id = "4681", Active = true, CountryId = "pt", ManagerId = "2010", FirstName = "Elias", LastName = "Sundaram", BirthDate = new DateOnly(1959, 4, 24), JoinDate = new DateOnly(2015, 10, 18) }
        , new EmployeeDTO() { Id = "4682", Active = true, CountryId = "uk", FirstName = "Clara", LastName = "Quant", BirthDate = new DateOnly(1980, 8, 18) }
        , new EmployeeDTO() { Id = "4684", Active = true, CountryId = "es", FirstName = "Diego", LastName = "Fragfton", BirthDate = new DateOnly(1964, 6, 16), JoinDate = new DateOnly(2005, 4, 24) }
        , new EmployeeDTO() { Id = "4686", Active = true, CountryId = "ie", FirstName = "Chelsea", LastName = "Dismore" }
        , new EmployeeDTO() { Id = "4687", CountryId = "za", ManagerId = "2008", FirstName = "Cassandra", LastName = "Paxton", BirthDate = new DateOnly(1961, 3, 21), JoinDate = new DateOnly(2008, 7, 14) }
        /*500*/
        , new EmployeeDTO() { Id = "4688", Active = true, CountryId = "pt", ManagerId = "2006", FirstName = "Cassandra", LastName = "Hudson", BirthDate = new DateOnly(1971, 2, 3) }
        , new EmployeeDTO() { Id = "4689", Active = true, CountryId = "usx", ManagerId = "2001", FirstName = "Dana", LastName = "Varkey", BirthDate = new DateOnly(1984, 10, 27), JoinDate = new DateOnly(2008, 7, 22) }
        , new EmployeeDTO() { Id = "4691", Active = true, CountryId = "es", ManagerId = "1002", FirstName = "Tobin", LastName = "Shanika", BirthDate = new DateOnly(1967, 9, 18), JoinDate = new DateOnly(2005, 7, 7), StartTime = new TimeOnly(17, 18, 44) }
        , new EmployeeDTO() { Id = "4693", Active = true, CountryId = "ae", ManagerId = "1001", FirstName = "Beverly", LastName = "Nordan", BirthDate = new DateOnly(1968, 4, 2), JoinDate = new DateOnly(2007, 7, 25), StartTime = new TimeOnly(14, 41, 48) }
        , new EmployeeDTO() { Id = "4694", CountryId = "za", FirstName = "Elena", LastName = "Robards", BirthDate = new DateOnly(1977, 3, 14), JoinDate = new DateOnly(2005, 1, 20) }
        /*505*/
        , new EmployeeDTO() { Id = "4695", Active = true, CountryId = "ca", ManagerId = "2011", FirstName = "Samar", LastName = "Towner", BirthDate = new DateOnly(1963, 11, 12) }
        , new EmployeeDTO() { Id = "4696", Active = true, CountryId = "ie", ManagerId = "2011", FirstName = "Ira", LastName = "Chinery", BirthDate = new DateOnly(1978, 11, 25) }
        , new EmployeeDTO() { Id = "4698", Active = true, CountryId = "uk", ManagerId = "2001", FirstName = "Thelma", LastName = "Godoy", BirthDate = new DateOnly(1970, 2, 10) }
        , new EmployeeDTO() { Id = "4699", Active = true, CountryId = "it", ManagerId = "2001", FirstName = "Amory", LastName = "Cadwell", BirthDate = new DateOnly(1982, 5, 23), JoinDate = new DateOnly(2013, 1, 7), StartTime = new TimeOnly(4, 42, 49) }
        , new EmployeeDTO() { Id = "4702", Active = true, CountryId = "it", FirstName = "Amory", LastName = "Aswell" }
        /*510*/
        , new EmployeeDTO() { Id = "4704", CountryId = "es", ManagerId = "2010", FirstName = "Rose", LastName = "Heavens", BirthDate = new DateOnly(1962, 10, 3), JoinDate = new DateOnly(2009, 11, 3) }
        , new EmployeeDTO() { Id = "4705", Active = true, CountryId = "us", ManagerId = "2005", FirstName = "Edney", LastName = "Tinsleay", BirthDate = new DateOnly(1982, 8, 9) }
        , new EmployeeDTO() { Id = "4708", Active = true, CountryId = "pt", FirstName = "Elron", LastName = "Warden", BirthDate = new DateOnly(1979, 11, 1) }
        , new EmployeeDTO() { Id = "4709", CountryId = "it", ManagerId = "1001", FirstName = "Brandon", LastName = "Jax", BirthDate = new DateOnly(1976, 6, 19), JoinDate = new DateOnly(2012, 4, 8) }
        , new EmployeeDTO() { Id = "4712", Active = true, CountryId = "it", FirstName = "Advikha", LastName = "Robey", BirthDate = new DateOnly(1972, 8, 12), JoinDate = new DateOnly(2008, 4, 10) }
        /*515*/
        , new EmployeeDTO() { Id = "4713", CountryId = "za", ManagerId = "2002", FirstName = "Charvi", LastName = "Jover", BirthDate = new DateOnly(1970, 9, 8) }
        , new EmployeeDTO() { Id = "4714", Active = true, CountryId = "es", ManagerId = "2006", FirstName = "Clara", LastName = "Cordner", BirthDate = new DateOnly(1983, 11, 21) }
        , new EmployeeDTO() { Id = "4716", Active = true, CountryId = "uk", ManagerId = "2001", FirstName = "Prisha", LastName = "Hamlin", BirthDate = new DateOnly(1973, 6, 21), JoinDate = new DateOnly(2015, 10, 5) }
        , new EmployeeDTO() { Id = "4717", Active = true, CountryId = "us", ManagerId = "2003", FirstName = "Elvira", LastName = "Broomell", BirthDate = new DateOnly(1977, 7, 19), JoinDate = new DateOnly(2015, 10, 5), StartTime = new TimeOnly(10, 13, 31) }
        , new EmployeeDTO() { Id = "4718", CountryId = "es", ManagerId = "2001", FirstName = "Cassandra", LastName = "Alarcon", BirthDate = new DateOnly(1971, 8, 22), JoinDate = new DateOnly(2015, 7, 23) }
        /*520*/
        , new EmployeeDTO() { Id = "4719", CountryId = "uk", FirstName = "Carter", LastName = "White", BirthDate = new DateOnly(1973, 8, 20), JoinDate = new DateOnly(2012, 11, 12) }
        , new EmployeeDTO() { Id = "4720", Active = true, CountryId = "ie", ManagerId = "1001", FirstName = "Rita", LastName = "Bravo", BirthDate = new DateOnly(1961, 8, 7) }
        , new EmployeeDTO() { Id = "4725", Active = true, CountryId = "ae", ManagerId = "2005", FirstName = "Elena", LastName = "Cadwell", BirthDate = new DateOnly(1962, 1, 2), JoinDate = new DateOnly(2016, 6, 12) }
        , new EmployeeDTO() { Id = "4726", Active = true, CountryId = "uk", ManagerId = "2003", FirstName = "Clara", LastName = "Hurley", BirthDate = new DateOnly(1964, 4, 24), JoinDate = new DateOnly(2010, 1, 11) }
        , new EmployeeDTO() { Id = "4728", Active = true, CountryId = "mx", FirstName = "Preston", LastName = "Nobble", BirthDate = new DateOnly(1959, 5, 17) }
        /*525*/
        , new EmployeeDTO() { Id = "4730", Active = true, CountryId = "pt", ManagerId = "2003", FirstName = "Sydney", LastName = "Lawthomtom", BirthDate = new DateOnly(1976, 2, 12), JoinDate = new DateOnly(2014, 9, 26) }
        , new EmployeeDTO() { Id = "4731", Active = true, CountryId = "uk", FirstName = "Emilio", LastName = "Robbs", BirthDate = new DateOnly(1980, 4, 16), JoinDate = new DateOnly(2015, 8, 24) }
        , new EmployeeDTO() { Id = "4732", Active = true, CountryId = "it", ManagerId = "2008", FirstName = "Jeremias", LastName = "Vohra", BirthDate = new DateOnly(1977, 7, 14), JoinDate = new DateOnly(2006, 8, 2) }
        , new EmployeeDTO() { Id = "4734", Active = true, CountryId = "us", ManagerId = "2006", FirstName = "Manuel", LastName = "Faulkner" }
        , new EmployeeDTO() { Id = "4735", Active = true, CountryId = "mx", FirstName = "Tobin", LastName = "Halwell", BirthDate = new DateOnly(1975, 2, 5), JoinDate = new DateOnly(2014, 7, 16) }
        /*530*/
        , new EmployeeDTO() { Id = "4737", Active = true, CountryId = "pt", ManagerId = "2011", FirstName = "Brandon", LastName = "Vohra", BirthDate = new DateOnly(1962, 8, 5), JoinDate = new DateOnly(2009, 1, 17) }
        , new EmployeeDTO() { Id = "4738", Active = true, CountryId = "us", ManagerId = "2003", FirstName = "Norton", LastName = "Batterton", BirthDate = new DateOnly(1973, 6, 15) }
        , new EmployeeDTO() { Id = "4739", Active = true, CountryId = "usx", FirstName = "John", LastName = "Meriwhether", BirthDate = new DateOnly(1966, 3, 2), JoinDate = new DateOnly(2014, 9, 21), StartTime = new TimeOnly(1, 10, 15) }
        , new EmployeeDTO() { Id = "4741", CountryId = "uk", ManagerId = "2003", FirstName = "Zara", LastName = "White", BirthDate = new DateOnly(1964, 2, 8), JoinDate = new DateOnly(2012, 10, 5), StartTime = new TimeOnly(1, 23, 54) }
        , new EmployeeDTO() { Id = "4743", Active = true, CountryId = "es", FirstName = "Samaira", LastName = "Batt", BirthDate = new DateOnly(1968, 3, 11), JoinDate = new DateOnly(2010, 4, 15), StartTime = new TimeOnly(11, 40, 36) }
        /*535*/
        , new EmployeeDTO() { Id = "4744", Active = true, CountryId = "usx", ManagerId = "2011", FirstName = "Enrique", LastName = "Silva" }
        , new EmployeeDTO() { Id = "4745", CountryId = "it", ManagerId = "2003", FirstName = "Charvi", LastName = "Shadow", BirthDate = new DateOnly(1964, 4, 17) }
        , new EmployeeDTO() { Id = "4746", Active = true, CountryId = "za", ManagerId = "2010", FirstName = "Javier", LastName = "Baxendale", BirthDate = new DateOnly(1966, 2, 21), JoinDate = new DateOnly(2014, 10, 24) }
        , new EmployeeDTO() { Id = "4747", CountryId = "pt", ManagerId = "2002", FirstName = "Chelsea", LastName = "Tinsleay", BirthDate = new DateOnly(1964, 7, 24), JoinDate = new DateOnly(2016, 1, 23), StartTime = new TimeOnly(1, 39, 11) }
        , new EmployeeDTO() { Id = "4749", Active = true, CountryId = "es", FirstName = "Bedford", LastName = "Chandra", BirthDate = new DateOnly(1975, 1, 13), JoinDate = new DateOnly(2012, 10, 19) }
        /*540*/
        , new EmployeeDTO() { Id = "4750", Active = true, CountryId = "za", ManagerId = "2002", FirstName = "Edric", LastName = "Bravo", BirthDate = new DateOnly(1972, 5, 7), JoinDate = new DateOnly(2012, 4, 3) }
        , new EmployeeDTO() { Id = "4751", Active = true, CountryId = "ca", ManagerId = "1002", FirstName = "Prisha", LastName = "Duque" }
        , new EmployeeDTO() { Id = "4752", Active = true, CountryId = "es", ManagerId = "2003", FirstName = "Emilio", LastName = "Ugarte", BirthDate = new DateOnly(1962, 11, 3), JoinDate = new DateOnly(2011, 4, 10) }
        , new EmployeeDTO() { Id = "4753", Active = true, CountryId = "es", ManagerId = "2003", FirstName = "Thelma", LastName = "Egerton", BirthDate = new DateOnly(1976, 5, 16), JoinDate = new DateOnly(2015, 4, 1), StartTime = new TimeOnly(0, 11, 27) }
        , new EmployeeDTO() { Id = "4754", Active = true, CountryId = "ca", ManagerId = "1002", FirstName = "Trenton", LastName = "Farham", BirthDate = new DateOnly(1975, 11, 12), JoinDate = new DateOnly(2013, 6, 1) }
        /*545*/
        , new EmployeeDTO() { Id = "4755", Active = true, CountryId = "us", ManagerId = "2010", FirstName = "Carter", LastName = "Astor", BirthDate = new DateOnly(1979, 7, 6), JoinDate = new DateOnly(2012, 1, 11) }
        , new EmployeeDTO() { Id = "4756", Active = true, CountryId = "uk", ManagerId = "2003", FirstName = "Elvira", LastName = "Beacham", BirthDate = new DateOnly(1978, 2, 3), JoinDate = new DateOnly(2007, 10, 25) }
        , new EmployeeDTO() { Id = "4757", Active = true, CountryId = "za", ManagerId = "2011", FirstName = "Diego", LastName = "Lee", BirthDate = new DateOnly(1965, 6, 21) }
        , new EmployeeDTO() { Id = "4758", CountryId = "ca", FirstName = "Dana", LastName = "Mitton", BirthDate = new DateOnly(1968, 6, 26), JoinDate = new DateOnly(2016, 7, 14), StartTime = new TimeOnly(13, 43, 17) }
        , new EmployeeDTO() { Id = "4759", CountryId = "ca", ManagerId = "2001", FirstName = "Elisa", LastName = "Cuny", BirthDate = new DateOnly(1983, 1, 19) }
        /*550*/
        , new EmployeeDTO() { Id = "4760", Active = true, CountryId = "uk", ManagerId = "2001", FirstName = "Manuel", LastName = "Quiroga", BirthDate = new DateOnly(1982, 11, 26), JoinDate = new DateOnly(2015, 9, 15) }
        , new EmployeeDTO() { Id = "4761", Active = true, CountryId = "es", ManagerId = "2002", FirstName = "Pricilla", LastName = "Bronson", BirthDate = new DateOnly(1980, 8, 2), JoinDate = new DateOnly(2011, 4, 22), StartTime = new TimeOnly(20, 41, 58) }
        , new EmployeeDTO() { Id = "4762", Active = true, CountryId = "usx", ManagerId = "2003", FirstName = "Ira", LastName = "Hammer", BirthDate = new DateOnly(1973, 4, 10), JoinDate = new DateOnly(2005, 10, 11) }
        , new EmployeeDTO() { Id = "4763", Active = true, CountryId = "ae", FirstName = "Navya", LastName = "Redding", BirthDate = new DateOnly(1959, 4, 17), JoinDate = new DateOnly(2010, 11, 3) }
        , new EmployeeDTO() { Id = "4765", Active = true, CountryId = "mx", ManagerId = "2002", FirstName = "Louise", LastName = "Paceiro", BirthDate = new DateOnly(1965, 6, 9) }
        /*555*/
        , new EmployeeDTO() { Id = "4766", CountryId = "ae", ManagerId = "2006", FirstName = "Eshan", LastName = "Crump", BirthDate = new DateOnly(1963, 6, 27), JoinDate = new DateOnly(2005, 4, 2), StartTime = new TimeOnly(19, 3, 21) }
        , new EmployeeDTO() { Id = "4768", Active = true, CountryId = "ae", ManagerId = "1002", FirstName = "Navya", LastName = "Duque", BirthDate = new DateOnly(1963, 4, 25), JoinDate = new DateOnly(2007, 1, 18) }
        , new EmployeeDTO() { Id = "4769", Active = true, CountryId = "mx", ManagerId = "2006", FirstName = "Barbara", LastName = "Lee", BirthDate = new DateOnly(1967, 10, 24) }
        , new EmployeeDTO() { Id = "4774", CountryId = "it", ManagerId = "1001", FirstName = "Andrea", LastName = "Norcutt", BirthDate = new DateOnly(1974, 1, 5), JoinDate = new DateOnly(2008, 9, 9) }
        , new EmployeeDTO() { Id = "4776", CountryId = "mx", ManagerId = "2006", FirstName = "Advikha", LastName = "Goudin", BirthDate = new DateOnly(1978, 6, 7) }
        /*560*/
        , new EmployeeDTO() { Id = "4777", Active = true, CountryId = "ca", ManagerId = "2002", FirstName = "Elron", LastName = "Pole", BirthDate = new DateOnly(1966, 10, 21) }
        , new EmployeeDTO() { Id = "4778", Active = true, CountryId = "ie", FirstName = "Balduino", LastName = "Wesley", BirthDate = new DateOnly(1963, 11, 14), JoinDate = new DateOnly(2015, 3, 17), StartTime = new TimeOnly(21, 46, 21) }
        , new EmployeeDTO() { Id = "4779", Active = true, CountryId = "mx", ManagerId = "2002", FirstName = "Edison", LastName = "Leaner", BirthDate = new DateOnly(1973, 5, 25) }
        , new EmployeeDTO() { Id = "4780", CountryId = "es", ManagerId = "2010", FirstName = "Eugenio", LastName = "Quant", BirthDate = new DateOnly(1969, 3, 3), JoinDate = new DateOnly(2014, 2, 2), StartTime = new TimeOnly(20, 17, 30) }
        , new EmployeeDTO() { Id = "4781", CountryId = "uk", FirstName = "Enrique", LastName = "Gadson", BirthDate = new DateOnly(1981, 10, 22), JoinDate = new DateOnly(2013, 1, 6), StartTime = new TimeOnly(17, 26, 16) }
        /*565*/
        , new EmployeeDTO() { Id = "4782", Active = true, CountryId = "uk", FirstName = "Javier", LastName = "Tinsleay", BirthDate = new DateOnly(1984, 5, 18), JoinDate = new DateOnly(2009, 4, 19) }
        , new EmployeeDTO() { Id = "4783", Active = true, CountryId = "uk", ManagerId = "2008", FirstName = "Zara", LastName = "Prest", BirthDate = new DateOnly(1969, 1, 17) }
        , new EmployeeDTO() { Id = "4784", Active = true, CountryId = "uk", ManagerId = "1001", FirstName = "Trenton", LastName = "Germano", BirthDate = new DateOnly(1972, 7, 12) }
        , new EmployeeDTO() { Id = "4786", Active = true, CountryId = "us", ManagerId = "2011", FirstName = "Chelsea", LastName = "Radley", BirthDate = new DateOnly(1970, 3, 10) }
        , new EmployeeDTO() { Id = "4789", CountryId = "ca", FirstName = "Rose", LastName = "Chesnut", BirthDate = new DateOnly(1962, 11, 6), JoinDate = new DateOnly(2009, 4, 26), StartTime = new TimeOnly(9, 0, 54) }
        /*570*/
        , new EmployeeDTO() { Id = "4790", CountryId = "ie", ManagerId = "2003", FirstName = "Cassandra", LastName = "Lee", BirthDate = new DateOnly(1968, 6, 16), JoinDate = new DateOnly(2013, 3, 24), StartTime = new TimeOnly(1, 39, 31) }
        , new EmployeeDTO() { Id = "4792", Active = true, CountryId = "uk", ManagerId = "1001", FirstName = "Carter", LastName = "Kallen", BirthDate = new DateOnly(1970, 2, 23), JoinDate = new DateOnly(2014, 2, 21), StartTime = new TimeOnly(5, 32, 20) }
        , new EmployeeDTO() { Id = "4794", Active = true, CountryId = "es", FirstName = "Rose", LastName = "Matel", BirthDate = new DateOnly(1965, 10, 16) }
        , new EmployeeDTO() { Id = "4795", CountryId = "ie", ManagerId = "1001", FirstName = "Mitul", LastName = "Diaz", BirthDate = new DateOnly(1963, 11, 9) }
        , new EmployeeDTO() { Id = "4796", Active = true, CountryId = "usx", FirstName = "Pricilla", LastName = "Fuster", BirthDate = new DateOnly(1982, 2, 21), JoinDate = new DateOnly(2015, 6, 15) }
        /*575*/
        , new EmployeeDTO() { Id = "4797", Active = true, CountryId = "es", FirstName = "Preston", LastName = "Halwell", BirthDate = new DateOnly(1982, 7, 5), JoinDate = new DateOnly(2009, 7, 3), StartTime = new TimeOnly(19, 48, 10) }
        , new EmployeeDTO() { Id = "4798", Active = true, CountryId = "ca", ManagerId = "2008", FirstName = "Carter", LastName = "Batton", BirthDate = new DateOnly(1980, 7, 18), JoinDate = new DateOnly(2013, 2, 7), StartTime = new TimeOnly(7, 43, 44) }
        , new EmployeeDTO() { Id = "4799", Active = true, CountryId = "us", ManagerId = "2002", FirstName = "Carter", LastName = "Lawes", BirthDate = new DateOnly(1966, 1, 11), JoinDate = new DateOnly(2005, 4, 27), StartTime = new TimeOnly(8, 25, 34) }
        , new EmployeeDTO() { Id = "4801", Active = true, CountryId = "mx", FirstName = "Blythe", LastName = "Varma", BirthDate = new DateOnly(1967, 8, 2), JoinDate = new DateOnly(2005, 3, 24), StartTime = new TimeOnly(3, 5, 20) }
        , new EmployeeDTO() { Id = "4802", Active = true, CountryId = "ie", ManagerId = "2005", FirstName = "Elena", LastName = "Bayley", BirthDate = new DateOnly(1978, 2, 25) }
        /*580*/
        , new EmployeeDTO() { Id = "4803", Active = true, CountryId = "pt", FirstName = "Chelsea", LastName = "Pitts", BirthDate = new DateOnly(1959, 4, 19) }
        , new EmployeeDTO() { Id = "4804", CountryId = "ca", ManagerId = "1002", FirstName = "Carter", LastName = "Gardner", BirthDate = new DateOnly(1965, 6, 15) }
        , new EmployeeDTO() { Id = "4806", Active = true, CountryId = "it", ManagerId = "1001", FirstName = "Rita", LastName = "Heaston", BirthDate = new DateOnly(1971, 7, 15) }
        , new EmployeeDTO() { Id = "4808", Active = true, CountryId = "mx", ManagerId = "1002", FirstName = "Prisha", LastName = "Fawcett", BirthDate = new DateOnly(1979, 8, 4) }
        , new EmployeeDTO() { Id = "4809", Active = true, CountryId = "usx", FirstName = "Pricilla", LastName = "Cuny", BirthDate = new DateOnly(1976, 1, 16), JoinDate = new DateOnly(2013, 6, 14), StartTime = new TimeOnly(19, 18, 31) }
        /*585*/
        , new EmployeeDTO() { Id = "4810", Active = true, CountryId = "za", ManagerId = "1002", FirstName = "Zara", LastName = "Brown", BirthDate = new DateOnly(1965, 5, 17), JoinDate = new DateOnly(2006, 6, 21), StartTime = new TimeOnly(18, 20, 42) }
        , new EmployeeDTO() { Id = "4813", Active = true, CountryId = "it", FirstName = "Prisha", LastName = "Ponte", BirthDate = new DateOnly(1961, 11, 10) }
        , new EmployeeDTO() { Id = "4814", Active = true, CountryId = "es", ManagerId = "1001", FirstName = "Rudolph", LastName = "Seaborn", BirthDate = new DateOnly(1962, 3, 11), JoinDate = new DateOnly(2010, 3, 20), StartTime = new TimeOnly(5, 29, 35) }
        , new EmployeeDTO() { Id = "4815", Active = true, CountryId = "mx", FirstName = "Elvira", LastName = "Lowden", BirthDate = new DateOnly(1973, 5, 4), JoinDate = new DateOnly(2008, 8, 17), StartTime = new TimeOnly(22, 13, 10) }
        , new EmployeeDTO() { Id = "4816", Active = true, CountryId = "usx", ManagerId = "2005", FirstName = "Navya", LastName = "Wayman", BirthDate = new DateOnly(1964, 2, 12) }
        /*590*/
        , new EmployeeDTO() { Id = "4817", Active = true, CountryId = "pt", ManagerId = "2008", FirstName = "Emilio", LastName = "Tinsleay", BirthDate = new DateOnly(1971, 9, 8) }
        , new EmployeeDTO() { Id = "4819", CountryId = "usx", ManagerId = "2011", FirstName = "Manuel", LastName = "Germano", BirthDate = new DateOnly(1968, 6, 12) }
        , new EmployeeDTO() { Id = "4820", Active = true, CountryId = "za", FirstName = "Prisha", LastName = "Quant", BirthDate = new DateOnly(1962, 6, 25), JoinDate = new DateOnly(2009, 6, 2) }
        , new EmployeeDTO() { Id = "4821", Active = true, CountryId = "mx", ManagerId = "2001", FirstName = "Cassandra", LastName = "Hilton", BirthDate = new DateOnly(1973, 1, 11), JoinDate = new DateOnly(2014, 3, 12), StartTime = new TimeOnly(1, 4, 30) }
        , new EmployeeDTO() { Id = "4822", Active = true, CountryId = "pt", ManagerId = "2005", FirstName = "Charvi", LastName = "Curt", BirthDate = new DateOnly(1975, 8, 19) }
        /*595*/
        , new EmployeeDTO() { Id = "4824", Active = true, CountryId = "ca", ManagerId = "2001", FirstName = "Rose", LastName = "Hudson", BirthDate = new DateOnly(1981, 5, 20) }
        , new EmployeeDTO() { Id = "4825", Active = true, CountryId = "pt", ManagerId = "1002", FirstName = "Mitul", LastName = "Towner", BirthDate = new DateOnly(1960, 10, 4), JoinDate = new DateOnly(2016, 5, 3) }
        , new EmployeeDTO() { Id = "4826", Active = true, CountryId = "uk", FirstName = "Cassandra", LastName = "Kenerly", BirthDate = new DateOnly(1982, 7, 7), JoinDate = new DateOnly(2015, 4, 25) }
        , new EmployeeDTO() { Id = "4828", Active = true, CountryId = "ca", ManagerId = "2006", FirstName = "Rudolph", LastName = "Hewitt", BirthDate = new DateOnly(1965, 7, 13) }
        , new EmployeeDTO() { Id = "4830", Active = true, CountryId = "es", ManagerId = "2008", FirstName = "Preston", LastName = "Ortega", BirthDate = new DateOnly(1964, 3, 12), JoinDate = new DateOnly(2013, 9, 22), StartTime = new TimeOnly(13, 13, 40) }
        /*600*/
        , new EmployeeDTO() { Id = "4831", Active = true, CountryId = "usx", ManagerId = "2002", FirstName = "Norton", LastName = "Pole", BirthDate = new DateOnly(1967, 1, 10), JoinDate = new DateOnly(2012, 7, 3) }
        , new EmployeeDTO() { Id = "4832", Active = true, CountryId = "pt", ManagerId = "2005", FirstName = "Rudolph", LastName = "Jax", BirthDate = new DateOnly(1969, 10, 14) }
        , new EmployeeDTO() { Id = "4836", Active = true, CountryId = "ie", FirstName = "Sahil", LastName = "Batton" }
        , new EmployeeDTO() { Id = "4838", Active = true, CountryId = "ie", ManagerId = "1002", FirstName = "Carter", LastName = "Kenerly", BirthDate = new DateOnly(1981, 9, 4), JoinDate = new DateOnly(2014, 11, 22) }
        , new EmployeeDTO() { Id = "4839", Active = true, CountryId = "ae", ManagerId = "2002", FirstName = "David", LastName = "Machado", BirthDate = new DateOnly(1973, 8, 10), JoinDate = new DateOnly(2012, 5, 23) }
        /*605*/
        , new EmployeeDTO() { Id = "4840", Active = true, CountryId = "uk", FirstName = "Coleman", LastName = "Lawthomtom", BirthDate = new DateOnly(1959, 2, 13), JoinDate = new DateOnly(2012, 2, 23) }
        , new EmployeeDTO() { Id = "4841", Active = true, CountryId = "ie", ManagerId = "2006", FirstName = "Sahil", LastName = "Godoy", BirthDate = new DateOnly(1961, 11, 12), JoinDate = new DateOnly(2009, 4, 13) }
        , new EmployeeDTO() { Id = "4842", Active = true, CountryId = "uk", ManagerId = "2001", FirstName = "Trenton", LastName = "Homes", BirthDate = new DateOnly(1978, 3, 24) }
        , new EmployeeDTO() { Id = "4843", Active = true, CountryId = "ie", FirstName = "Dana", LastName = "Robards", BirthDate = new DateOnly(1962, 8, 14), JoinDate = new DateOnly(2012, 4, 26), StartTime = new TimeOnly(11, 43, 40) }
        , new EmployeeDTO() { Id = "4844", Active = true, CountryId = "uk", ManagerId = "1002", FirstName = "Advikha", LastName = "Hilton", BirthDate = new DateOnly(1972, 5, 26) }
        /*610*/
        , new EmployeeDTO() { Id = "4848", Active = true, CountryId = "ie", ManagerId = "2006", FirstName = "Rudolph", LastName = "Chinery", BirthDate = new DateOnly(1974, 6, 9) }
        , new EmployeeDTO() { Id = "4850", Active = true, CountryId = "ca", FirstName = "Elisa", LastName = "Checa", BirthDate = new DateOnly(1972, 5, 8), JoinDate = new DateOnly(2016, 5, 23), StartTime = new TimeOnly(18, 23, 58) }
        , new EmployeeDTO() { Id = "4851", Active = true, CountryId = "it", ManagerId = "2001", FirstName = "Diego", LastName = "Hamlin", BirthDate = new DateOnly(1968, 10, 18) }
        , new EmployeeDTO() { Id = "4852", Active = true, CountryId = "ae", FirstName = "Amory", LastName = "Esparza", BirthDate = new DateOnly(1982, 6, 11), JoinDate = new DateOnly(2016, 7, 2) }
        , new EmployeeDTO() { Id = "4854", Active = true, CountryId = "es", ManagerId = "2001", FirstName = "Rita", LastName = "Lowden", BirthDate = new DateOnly(1971, 11, 6) }
        /*615*/
        , new EmployeeDTO() { Id = "4855", Active = true, CountryId = "ca", FirstName = "Wilbur", LastName = "Shanika", BirthDate = new DateOnly(1983, 10, 25), JoinDate = new DateOnly(2008, 4, 3), StartTime = new TimeOnly(4, 26, 11) }
        , new EmployeeDTO() { Id = "4857", Active = true, CountryId = "us", ManagerId = "2005", FirstName = "Samar", LastName = "Sinyard", BirthDate = new DateOnly(1959, 1, 5) }
        , new EmployeeDTO() { Id = "4858", Active = true, CountryId = "usx", FirstName = "Blythe", LastName = "Alarcon", BirthDate = new DateOnly(1978, 10, 6), JoinDate = new DateOnly(2013, 3, 22), StartTime = new TimeOnly(2, 37, 1) }
        , new EmployeeDTO() { Id = "4860", Active = true, CountryId = "us", ManagerId = "2011", FirstName = "Thelma", LastName = "Atcher", BirthDate = new DateOnly(1969, 9, 26) }
        , new EmployeeDTO() { Id = "4861", Active = true, CountryId = "ie", FirstName = "Manuel", LastName = "Jamarcus", BirthDate = new DateOnly(1962, 7, 13), JoinDate = new DateOnly(2009, 3, 13), StartTime = new TimeOnly(12, 35, 25) }
        /*620*/
        , new EmployeeDTO() { Id = "4862", Active = true, CountryId = "mx", ManagerId = "2002", FirstName = "Charvi", LastName = "Waylon", BirthDate = new DateOnly(1960, 5, 2) }
        , new EmployeeDTO() { Id = "4863", Active = true, CountryId = "ca", FirstName = "Prynash", LastName = "Moke", BirthDate = new DateOnly(1968, 3, 17) }
        , new EmployeeDTO() { Id = "4864", Active = true, CountryId = "uk", ManagerId = "2005", FirstName = "Eduardo", LastName = "Hoit", BirthDate = new DateOnly(1974, 9, 9), JoinDate = new DateOnly(2005, 3, 5) }
        , new EmployeeDTO() { Id = "4865", Active = true, CountryId = "ie", ManagerId = "2002", FirstName = "Tobin", LastName = "Chandra", BirthDate = new DateOnly(1964, 3, 21), JoinDate = new DateOnly(2016, 10, 23) }
        , new EmployeeDTO() { Id = "4866", CountryId = "es", ManagerId = "2006", FirstName = "Sahil", LastName = "Hartwell", BirthDate = new DateOnly(1967, 8, 11) }
        /*625*/
        , new EmployeeDTO() { Id = "4868", Active = true, CountryId = "usx", ManagerId = "2008", FirstName = "Louise", LastName = "Jover", BirthDate = new DateOnly(1962, 11, 1), JoinDate = new DateOnly(2007, 10, 2) }
        , new EmployeeDTO() { Id = "4869", Active = true, CountryId = "ae", ManagerId = "2008", FirstName = "Cassandra", LastName = "Calloway", BirthDate = new DateOnly(1962, 5, 8) }
        , new EmployeeDTO() { Id = "4870", Active = true, CountryId = "ie", ManagerId = "2003", FirstName = "Ernesto", LastName = "Germano" }
        , new EmployeeDTO() { Id = "4871", CountryId = "ae", ManagerId = "2005", FirstName = "Brandon", LastName = "Chandra" }
        , new EmployeeDTO() { Id = "4872", Active = true, CountryId = "es", ManagerId = "2011", FirstName = "Chelsea", LastName = "Platten", BirthDate = new DateOnly(1959, 7, 1) }
        /*630*/
        , new EmployeeDTO() { Id = "4873", CountryId = "it", ManagerId = "2001", FirstName = "Blythe", LastName = "Norcutt", BirthDate = new DateOnly(1982, 7, 1), JoinDate = new DateOnly(2013, 6, 16), StartTime = new TimeOnly(11, 48, 9) }
        , new EmployeeDTO() { Id = "4875", CountryId = "usx", ManagerId = "2002", FirstName = "Ernesto", LastName = "Batt", BirthDate = new DateOnly(1976, 3, 18), JoinDate = new DateOnly(2006, 5, 3), StartTime = new TimeOnly(22, 34, 26) }
        , new EmployeeDTO() { Id = "4877", Active = true, CountryId = "ae", ManagerId = "1002", FirstName = "Amory", LastName = "Camino", BirthDate = new DateOnly(1967, 6, 15) }
        , new EmployeeDTO() { Id = "4880", Active = true, CountryId = "it", FirstName = "Trenton", LastName = "Paceiro", BirthDate = new DateOnly(1982, 7, 27), JoinDate = new DateOnly(2016, 3, 13), StartTime = new TimeOnly(6, 52, 21) }
        , new EmployeeDTO() { Id = "4884", Active = true, CountryId = "us", ManagerId = "2008", FirstName = "Louise", LastName = "Millan", BirthDate = new DateOnly(1982, 2, 4), JoinDate = new DateOnly(2010, 9, 7), StartTime = new TimeOnly(11, 49, 24) }
        /*635*/
        , new EmployeeDTO() { Id = "4885", Active = true, CountryId = "es", ManagerId = "2011", FirstName = "Cassandra", LastName = "Ugarte", BirthDate = new DateOnly(1975, 5, 16), JoinDate = new DateOnly(2006, 5, 16) }
        , new EmployeeDTO() { Id = "4886", Active = true, CountryId = "pt", ManagerId = "2011", FirstName = "Thelma", LastName = "Alarcon", BirthDate = new DateOnly(1980, 2, 27), JoinDate = new DateOnly(2005, 10, 24) }
        , new EmployeeDTO() { Id = "4887", Active = true, CountryId = "ae", ManagerId = "2001", FirstName = "John", LastName = "Faulkner", BirthDate = new DateOnly(1981, 2, 14) }
        , new EmployeeDTO() { Id = "4891", Active = true, CountryId = "pt", ManagerId = "2002", FirstName = "Cassandra", LastName = "Milton" }
        , new EmployeeDTO() { Id = "4893", Active = true, CountryId = "pt", ManagerId = "2011", FirstName = "Eugenio", LastName = "Hankin", BirthDate = new DateOnly(1961, 11, 2), JoinDate = new DateOnly(2009, 11, 18), StartTime = new TimeOnly(19, 57, 51) }
        /*640*/
        , new EmployeeDTO() { Id = "4894", Active = true, CountryId = "ca", ManagerId = "1002", FirstName = "Tobin", LastName = "Silva", BirthDate = new DateOnly(1974, 2, 6) }
        , new EmployeeDTO() { Id = "4895", Active = true, CountryId = "es", ManagerId = "2005", FirstName = "Elron", LastName = "Alarcon" }
        , new EmployeeDTO() { Id = "4897", Active = true, CountryId = "ae", FirstName = "Tobin", LastName = "Keller", BirthDate = new DateOnly(1974, 5, 11) }
        , new EmployeeDTO() { Id = "4900", Active = true, CountryId = "it", ManagerId = "2006", FirstName = "Carter", LastName = "Gardner", BirthDate = new DateOnly(1981, 4, 9) }
        , new EmployeeDTO() { Id = "4902", Active = true, CountryId = "ca", ManagerId = "2002", FirstName = "Edney", LastName = "Adams", BirthDate = new DateOnly(1962, 2, 1), JoinDate = new DateOnly(2005, 1, 21) }
        /*645*/
        , new EmployeeDTO() { Id = "4906", Active = true, CountryId = "us", ManagerId = "2003", FirstName = "Charvi", LastName = "Talton", BirthDate = new DateOnly(1974, 7, 15) }
        , new EmployeeDTO() { Id = "4908", CountryId = "es", ManagerId = "2010", FirstName = "Advikha", LastName = "Broomell", BirthDate = new DateOnly(1966, 7, 13) }
        , new EmployeeDTO() { Id = "4910", Active = true, CountryId = "uk", ManagerId = "1001", FirstName = "Beverly", LastName = "Asuncion", BirthDate = new DateOnly(1973, 4, 13) }
        , new EmployeeDTO() { Id = "4911", Active = true, CountryId = "es", ManagerId = "2008", FirstName = "Pricilla", LastName = "Batterson" }
        , new EmployeeDTO() { Id = "4912", Active = true, CountryId = "us", ManagerId = "1001", FirstName = "Rudolph", LastName = "Meriwhether", BirthDate = new DateOnly(1961, 9, 16), JoinDate = new DateOnly(2009, 11, 27) }
        /*650*/
        , new EmployeeDTO() { Id = "4913", Active = true, CountryId = "us", FirstName = "Andrea", LastName = "Machado", BirthDate = new DateOnly(1959, 3, 21), JoinDate = new DateOnly(2005, 11, 25) }
        , new EmployeeDTO() { Id = "4914", Active = true, CountryId = "ae", ManagerId = "2001", FirstName = "Manuel", LastName = "Machado", BirthDate = new DateOnly(1975, 6, 15) }
        , new EmployeeDTO() { Id = "4915", Active = true, CountryId = "za", FirstName = "John", LastName = "Norcutt", BirthDate = new DateOnly(1982, 8, 23), JoinDate = new DateOnly(2006, 3, 27), StartTime = new TimeOnly(13, 4, 3) }
        , new EmployeeDTO() { Id = "4916", Active = true, CountryId = "pt", ManagerId = "2005", FirstName = "Eshan", LastName = "Edger" }
        , new EmployeeDTO() { Id = "4918", Active = true, CountryId = "ca", ManagerId = "2002", FirstName = "Prisha", LastName = "Segarra", BirthDate = new DateOnly(1980, 7, 16), JoinDate = new DateOnly(2012, 9, 26) }
        /*655*/
        , new EmployeeDTO() { Id = "4919", Active = true, CountryId = "ae", ManagerId = "1002", FirstName = "Balduino", LastName = "Teelford", BirthDate = new DateOnly(1965, 3, 5), JoinDate = new DateOnly(2013, 10, 14), StartTime = new TimeOnly(20, 52, 9) }
        , new EmployeeDTO() { Id = "4920", CountryId = "ae", ManagerId = "2008", FirstName = "Dana", LastName = "Zola", BirthDate = new DateOnly(1976, 1, 18) }
        , new EmployeeDTO() { Id = "4921", Active = true, CountryId = "ca", FirstName = "Rudolph", LastName = "Nordan" }
        , new EmployeeDTO() { Id = "4922", CountryId = "us", ManagerId = "1001", FirstName = "Brandon", LastName = "Machado", BirthDate = new DateOnly(1961, 9, 19) }
        , new EmployeeDTO() { Id = "4925", Active = true, CountryId = "us", ManagerId = "2008", FirstName = "Navya", LastName = "Batalla" }
        /*660*/
        , new EmployeeDTO() { Id = "4926", Active = true, CountryId = "uk", FirstName = "Norton", LastName = "White", BirthDate = new DateOnly(1969, 1, 5) }
        , new EmployeeDTO() { Id = "4927", Active = true, CountryId = "ae", FirstName = "Beverly", LastName = "Edger", BirthDate = new DateOnly(1981, 5, 22) }
        , new EmployeeDTO() { Id = "4929", Active = true, CountryId = "ca", ManagerId = "2008", FirstName = "Tina", LastName = "Simons", BirthDate = new DateOnly(1979, 8, 17), JoinDate = new DateOnly(2009, 11, 17) }
        , new EmployeeDTO() { Id = "4930", Active = true, CountryId = "ca", FirstName = "Rudolph", LastName = "Richarson", BirthDate = new DateOnly(1978, 4, 14), JoinDate = new DateOnly(2014, 9, 12) }
        , new EmployeeDTO() { Id = "4931", Active = true, CountryId = "ie", ManagerId = "2005", FirstName = "Ainsley", LastName = "Hartwell", BirthDate = new DateOnly(1978, 1, 27), JoinDate = new DateOnly(2012, 4, 22) }
        /*665*/
        , new EmployeeDTO() { Id = "4933", Active = true, CountryId = "za", ManagerId = "2010", FirstName = "Elisa", LastName = "Sunshine" }
        , new EmployeeDTO() { Id = "4934", Active = true, CountryId = "ca", FirstName = "Sydney", LastName = "Silva", BirthDate = new DateOnly(1968, 3, 2), JoinDate = new DateOnly(2016, 4, 11) }
        , new EmployeeDTO() { Id = "4935", Active = true, CountryId = "ca", ManagerId = "2008", FirstName = "Ainsley", LastName = "Donns", BirthDate = new DateOnly(1978, 9, 4) }
        , new EmployeeDTO() { Id = "4937", CountryId = "it", ManagerId = "2010", FirstName = "Enrique", LastName = "Crump", BirthDate = new DateOnly(1961, 1, 9) }
        , new EmployeeDTO() { Id = "4938", Active = true, CountryId = "pt", FirstName = "Elena", LastName = "Redman", BirthDate = new DateOnly(1982, 9, 10), JoinDate = new DateOnly(2005, 9, 21), StartTime = new TimeOnly(1, 2, 19) }
        /*670*/
        , new EmployeeDTO() { Id = "4939", CountryId = "za", FirstName = "Dana", LastName = "Bronson", BirthDate = new DateOnly(1980, 5, 3) }
        , new EmployeeDTO() { Id = "4940", CountryId = "es", ManagerId = "2001", FirstName = "Enrique", LastName = "Lavendeer" }
        , new EmployeeDTO() { Id = "4941", Active = true, CountryId = "ie", ManagerId = "2006", FirstName = "Rose", LastName = "Saxe", BirthDate = new DateOnly(1959, 1, 1) }
        , new EmployeeDTO() { Id = "4942", Active = true, CountryId = "us", ManagerId = "1002", FirstName = "Elisa", LastName = "Wesley", BirthDate = new DateOnly(1970, 6, 19) }
        , new EmployeeDTO() { Id = "4944", CountryId = "pt", FirstName = "Preston", LastName = "Zola", BirthDate = new DateOnly(1970, 8, 2), JoinDate = new DateOnly(2005, 2, 5) }
        /*675*/
        , new EmployeeDTO() { Id = "4945", Active = true, CountryId = "pt", ManagerId = "2010", FirstName = "Elias", LastName = "Ortega", BirthDate = new DateOnly(1968, 1, 2), JoinDate = new DateOnly(2007, 10, 20), StartTime = new TimeOnly(22, 47, 29) }
        , new EmployeeDTO() { Id = "4950", Active = true, CountryId = "uk", ManagerId = "2001", FirstName = "Rudolph", LastName = "Mitton", BirthDate = new DateOnly(1984, 4, 3), JoinDate = new DateOnly(2006, 7, 26), StartTime = new TimeOnly(21, 35, 28) }
        , new EmployeeDTO() { Id = "4951", Active = true, CountryId = "za", ManagerId = "2006", FirstName = "Louise", LastName = "Pinson", BirthDate = new DateOnly(1964, 8, 13), JoinDate = new DateOnly(2014, 3, 24), StartTime = new TimeOnly(21, 42, 55) }
        , new EmployeeDTO() { Id = "4952", CountryId = "ca", ManagerId = "1002", FirstName = "Jeremias", LastName = "Richarson", BirthDate = new DateOnly(1973, 1, 21), JoinDate = new DateOnly(2006, 8, 23), StartTime = new TimeOnly(9, 22, 43) }
        , new EmployeeDTO() { Id = "4954", Active = true, CountryId = "za", ManagerId = "2003", FirstName = "Preston", LastName = "Vyass", BirthDate = new DateOnly(1982, 9, 21), JoinDate = new DateOnly(2007, 8, 17) }
        /*680*/
        , new EmployeeDTO() { Id = "4955", Active = true, CountryId = "ae", ManagerId = "2001", FirstName = "Prynash", LastName = "Newman", BirthDate = new DateOnly(1970, 2, 9) }
        , new EmployeeDTO() { Id = "4956", Active = true, CountryId = "it", ManagerId = "2001", FirstName = "Thelma", LastName = "Baylor", BirthDate = new DateOnly(1960, 1, 20), JoinDate = new DateOnly(2011, 8, 20) }
        , new EmployeeDTO() { Id = "4957", Active = true, CountryId = "ca", ManagerId = "1001", FirstName = "Samaira", LastName = "Christon", BirthDate = new DateOnly(1981, 7, 27), JoinDate = new DateOnly(2006, 6, 7) }
        , new EmployeeDTO() { Id = "4958", CountryId = "mx", FirstName = "Sydney", LastName = "Locke", BirthDate = new DateOnly(1967, 9, 23) }
        , new EmployeeDTO() { Id = "4959", Active = true, CountryId = "es", FirstName = "Eduardo", LastName = "Faulkner", BirthDate = new DateOnly(1980, 10, 25), JoinDate = new DateOnly(2014, 6, 10), StartTime = new TimeOnly(14, 0, 56) }
        /*685*/
        , new EmployeeDTO() { Id = "4960", CountryId = "za", ManagerId = "2011", FirstName = "Ainsley", LastName = "Parkinson", BirthDate = new DateOnly(1959, 6, 20), JoinDate = new DateOnly(2011, 8, 16) }
        , new EmployeeDTO() { Id = "4961", CountryId = "it", ManagerId = "2008", FirstName = "Chelsea", LastName = "Fenning", BirthDate = new DateOnly(1969, 6, 13), JoinDate = new DateOnly(2005, 4, 9) }
        , new EmployeeDTO() { Id = "4963", Active = true, CountryId = "it", ManagerId = "1001", FirstName = "Dana", LastName = "Risley", BirthDate = new DateOnly(1968, 2, 17) }
        , new EmployeeDTO() { Id = "4964", Active = true, CountryId = "us", ManagerId = "2003", FirstName = "Trenton", LastName = "Kenerly", BirthDate = new DateOnly(1959, 3, 6), JoinDate = new DateOnly(2005, 10, 8) }
        , new EmployeeDTO() { Id = "4965", Active = true, CountryId = "us", FirstName = "Emilio", LastName = "Baver", BirthDate = new DateOnly(1976, 3, 22) }
        /*690*/
        , new EmployeeDTO() { Id = "4967", Active = true, CountryId = "ie", ManagerId = "2011", FirstName = "Thelma", LastName = "Hernando", BirthDate = new DateOnly(1980, 6, 12), JoinDate = new DateOnly(2006, 4, 12), StartTime = new TimeOnly(19, 40, 45) }
        , new EmployeeDTO() { Id = "4969", CountryId = "uk", ManagerId = "2006", FirstName = "Ainsley", LastName = "Bayley", BirthDate = new DateOnly(1977, 1, 10) }
        , new EmployeeDTO() { Id = "4970", Active = true, CountryId = "mx", FirstName = "Diego", LastName = "Sise", BirthDate = new DateOnly(1976, 10, 21), JoinDate = new DateOnly(2005, 4, 6) }
        , new EmployeeDTO() { Id = "4971", Active = true, CountryId = "ae", ManagerId = "1001", FirstName = "Coleman", LastName = "Sundaram", BirthDate = new DateOnly(1962, 5, 13), JoinDate = new DateOnly(2006, 5, 3), StartTime = new TimeOnly(12, 17, 26) }
        , new EmployeeDTO() { Id = "4973", Active = true, CountryId = "uk", ManagerId = "2008", FirstName = "Stuart", LastName = "Fabra" }
        /*695*/
        , new EmployeeDTO() { Id = "4974", CountryId = "mx", FirstName = "Samaira", LastName = "Baver", BirthDate = new DateOnly(1983, 3, 1), JoinDate = new DateOnly(2009, 8, 14) }
        , new EmployeeDTO() { Id = "4975", Active = true, CountryId = "ie", ManagerId = "1001", FirstName = "Dana", LastName = "Hewitt", BirthDate = new DateOnly(1983, 3, 22), JoinDate = new DateOnly(2012, 10, 13), StartTime = new TimeOnly(12, 10, 1) }
        , new EmployeeDTO() { Id = "4976", Active = true, CountryId = "pt", FirstName = "Charvi", LastName = "Halwell", BirthDate = new DateOnly(1966, 9, 1) }
        , new EmployeeDTO() { Id = "4977", Active = true, CountryId = "it", ManagerId = "1002", FirstName = "Emilio", LastName = "Cornwell", BirthDate = new DateOnly(1970, 1, 6) }
        , new EmployeeDTO() { Id = "4978", Active = true, CountryId = "ca", FirstName = "Edney", LastName = "Black", BirthDate = new DateOnly(1979, 11, 27), JoinDate = new DateOnly(2012, 8, 15) }
        /*700*/
        , new EmployeeDTO() { Id = "4983", Active = true, CountryId = "ca", ManagerId = "2008", FirstName = "Rita", LastName = "Talton", BirthDate = new DateOnly(1979, 4, 12), JoinDate = new DateOnly(2015, 6, 27) }
        , new EmployeeDTO() { Id = "4985", Active = true, CountryId = "mx", FirstName = "Elena", LastName = "Bora", BirthDate = new DateOnly(1966, 6, 14), JoinDate = new DateOnly(2014, 10, 25) }
        , new EmployeeDTO() { Id = "4987", CountryId = "es", FirstName = "Ira", LastName = "Vohra", BirthDate = new DateOnly(1977, 6, 14), JoinDate = new DateOnly(2012, 2, 19) }
        , new EmployeeDTO() { Id = "4989", Active = true, CountryId = "usx", FirstName = "Louise", LastName = "Atchley", BirthDate = new DateOnly(1984, 6, 5) }
        , new EmployeeDTO() { Id = "4990", Active = true, CountryId = "mx", ManagerId = "2003", FirstName = "Wilbur", LastName = "Hewitt", BirthDate = new DateOnly(1974, 11, 5), JoinDate = new DateOnly(2012, 1, 26) }
        /*705*/
        , new EmployeeDTO() { Id = "4991", Active = true, CountryId = "ie", ManagerId = "2011", FirstName = "Prynash", LastName = "Ortega", BirthDate = new DateOnly(1962, 11, 27), JoinDate = new DateOnly(2009, 1, 27) }
        , new EmployeeDTO() { Id = "4992", Active = true, CountryId = "it", ManagerId = "2005", FirstName = "Carter", LastName = "Hammer", BirthDate = new DateOnly(1973, 8, 15) }
        , new EmployeeDTO() { Id = "4993", Active = true, CountryId = "ie", ManagerId = "2010", FirstName = "Blythe", LastName = "Batterton", BirthDate = new DateOnly(1983, 10, 6) }
        , new EmployeeDTO() { Id = "4994", Active = true, CountryId = "mx", ManagerId = "2002", FirstName = "Tina", LastName = "Fleet", BirthDate = new DateOnly(1974, 1, 27), JoinDate = new DateOnly(2011, 5, 24) }
        , new EmployeeDTO() { Id = "4996", Active = true, CountryId = "usx", ManagerId = "2001", FirstName = "Diego", LastName = "Nakisha" }
        /*710*/
        , new EmployeeDTO() { Id = "4997", Active = true, CountryId = "es", ManagerId = "1002", FirstName = "Elias", LastName = "Faulkner", BirthDate = new DateOnly(1973, 3, 19), JoinDate = new DateOnly(2008, 4, 21), StartTime = new TimeOnly(10, 19, 51) }
        , new EmployeeDTO() { Id = "4998", Active = true, CountryId = "us", ManagerId = "2011", FirstName = "Enrique", LastName = "Barranco", BirthDate = new DateOnly(1975, 3, 3) }
        , new EmployeeDTO() { Id = "4999", Active = true, CountryId = "ie", FirstName = "Norton", LastName = "Pulhart", BirthDate = new DateOnly(1982, 3, 11) }
        , new EmployeeDTO() { Id = "5000", Active = true, CountryId = "it", FirstName = "Mitul", LastName = "Beacham", BirthDate = new DateOnly(1964, 8, 16) }
        , new EmployeeDTO() { Id = "5001", Active = true, CountryId = "uk", ManagerId = "1002", FirstName = "Rita", LastName = "Wayman", BirthDate = new DateOnly(1979, 7, 26) }
        /*715*/
        , new EmployeeDTO() { Id = "5002", CountryId = "za", ManagerId = "2005", FirstName = "Antonio", LastName = "Machado" }
        , new EmployeeDTO() { Id = "5003", CountryId = "ae", FirstName = "Rita", LastName = "Godoy", BirthDate = new DateOnly(1965, 9, 24), JoinDate = new DateOnly(2007, 7, 14) }
        , new EmployeeDTO() { Id = "5004", CountryId = "uk", FirstName = "Blythe", LastName = "Robbs", BirthDate = new DateOnly(1979, 1, 2), JoinDate = new DateOnly(2008, 7, 14), StartTime = new TimeOnly(1, 35, 29) }
        , new EmployeeDTO() { Id = "5005", CountryId = "usx", ManagerId = "1002", FirstName = "Charvi", LastName = "Hernando" }
        , new EmployeeDTO() { Id = "5006", Active = true, CountryId = "mx", FirstName = "Prynash", LastName = "Esparza", BirthDate = new DateOnly(1959, 3, 24) }
        /*720*/
        , new EmployeeDTO() { Id = "5009", Active = true, CountryId = "ie", FirstName = "Advikha", LastName = "Shanika", BirthDate = new DateOnly(1963, 11, 1) }
        , new EmployeeDTO() { Id = "5010", Active = true, CountryId = "ae", ManagerId = "2002", FirstName = "Ainsley", LastName = "Vyass" }
        , new EmployeeDTO() { Id = "5011", Active = true, CountryId = "it", ManagerId = "2010", FirstName = "Trenton", LastName = "Black", BirthDate = new DateOnly(1963, 10, 20) }
        , new EmployeeDTO() { Id = "5012", Active = true, CountryId = "es", ManagerId = "1001", FirstName = "Bedford", LastName = "Ranger", BirthDate = new DateOnly(1973, 9, 5), JoinDate = new DateOnly(2008, 7, 13) }
        , new EmployeeDTO() { Id = "5013", Active = true, CountryId = "mx", FirstName = "Blythe", LastName = "Dipple", BirthDate = new DateOnly(1967, 9, 1) }
        /*725*/
        , new EmployeeDTO() { Id = "5014", Active = true, CountryId = "mx", ManagerId = "2002", FirstName = "Stuart", LastName = "Infante", BirthDate = new DateOnly(1978, 8, 5) }
        , new EmployeeDTO() { Id = "5015", CountryId = "uk", ManagerId = "1001", FirstName = "Edney", LastName = "Hampston", BirthDate = new DateOnly(1984, 11, 11), JoinDate = new DateOnly(2012, 4, 16), StartTime = new TimeOnly(16, 48, 20) }
        , new EmployeeDTO() { Id = "5016", CountryId = "ae", FirstName = "Charvi", LastName = "Ater", BirthDate = new DateOnly(1964, 5, 7), JoinDate = new DateOnly(2012, 7, 22) }
        , new EmployeeDTO() { Id = "5017", Active = true, CountryId = "za", ManagerId = "2011", FirstName = "Tobin", LastName = "Shadow", BirthDate = new DateOnly(1959, 8, 14), JoinDate = new DateOnly(2011, 1, 23) }
        , new EmployeeDTO() { Id = "5018", Active = true, CountryId = "usx", ManagerId = "2008", FirstName = "Chelsea", LastName = "Predmore", BirthDate = new DateOnly(1973, 6, 19), JoinDate = new DateOnly(2013, 1, 15), StartTime = new TimeOnly(0, 48, 4) }
        /*730*/
        , new EmployeeDTO() { Id = "5021", Active = true, CountryId = "ca", FirstName = "Chelsea", LastName = "Batalla", BirthDate = new DateOnly(1975, 5, 11), JoinDate = new DateOnly(2006, 3, 23) }
        , new EmployeeDTO() { Id = "5023", Active = true, CountryId = "us", ManagerId = "2005", FirstName = "Clara", LastName = "Beacham", BirthDate = new DateOnly(1974, 11, 11), JoinDate = new DateOnly(2007, 9, 26) }
        , new EmployeeDTO() { Id = "5026", CountryId = "ae", ManagerId = "2002", FirstName = "Trenton", LastName = "Machado", BirthDate = new DateOnly(1979, 7, 23) }
        , new EmployeeDTO() { Id = "5027", Active = true, CountryId = "us", ManagerId = "2003", FirstName = "Eduardo", LastName = "Vohra", BirthDate = new DateOnly(1968, 7, 3), JoinDate = new DateOnly(2012, 1, 5) }
        , new EmployeeDTO() { Id = "5029", CountryId = "ca", FirstName = "David", LastName = "Edger", BirthDate = new DateOnly(1965, 3, 12) }
        /*735*/
        , new EmployeeDTO() { Id = "5030", Active = true, CountryId = "us", ManagerId = "2011", FirstName = "Norton", LastName = "Curt", BirthDate = new DateOnly(1962, 11, 15), JoinDate = new DateOnly(2014, 11, 25) }
        , new EmployeeDTO() { Id = "5031", CountryId = "pt", ManagerId = "2005", FirstName = "Wilbur", LastName = "Beacham" }
        , new EmployeeDTO() { Id = "5032", Active = true, CountryId = "ie", FirstName = "Stuart", LastName = "Hilton", BirthDate = new DateOnly(1972, 9, 10) }
        , new EmployeeDTO() { Id = "5033", CountryId = "pt", ManagerId = "2008", FirstName = "Ernesto", LastName = "Hewitt", BirthDate = new DateOnly(1965, 10, 4), JoinDate = new DateOnly(2015, 4, 9) }
        , new EmployeeDTO() { Id = "5036", CountryId = "us", ManagerId = "2010", FirstName = "Elvira", LastName = "Radley", BirthDate = new DateOnly(1966, 1, 8), JoinDate = new DateOnly(2011, 10, 6) }
        /*740*/
        , new EmployeeDTO() { Id = "5037", Active = true, CountryId = "uk", ManagerId = "2010", FirstName = "Dana", LastName = "Ater", BirthDate = new DateOnly(1981, 6, 24), JoinDate = new DateOnly(2016, 6, 2), StartTime = new TimeOnly(4, 36, 32) }
        , new EmployeeDTO() { Id = "5039", Active = true, CountryId = "usx", FirstName = "Andrea", LastName = "Simons", BirthDate = new DateOnly(1974, 2, 19) }
        , new EmployeeDTO() { Id = "5041", Active = true, CountryId = "pt", FirstName = "Eduardo", LastName = "Kurtis", BirthDate = new DateOnly(1980, 8, 13), JoinDate = new DateOnly(2008, 6, 7), StartTime = new TimeOnly(7, 13, 38) }
        , new EmployeeDTO() { Id = "5042", Active = true, CountryId = "uk", ManagerId = "2003", FirstName = "Coleman", LastName = "Fleet", BirthDate = new DateOnly(1960, 4, 10), JoinDate = new DateOnly(2014, 3, 13), StartTime = new TimeOnly(16, 31, 3) }
        , new EmployeeDTO() { Id = "5043", Active = true, CountryId = "ae", ManagerId = "2006", FirstName = "Balduino", LastName = "Beach", BirthDate = new DateOnly(1978, 6, 7) }
        /*745*/
        , new EmployeeDTO() { Id = "5045", Active = true, CountryId = "ae", ManagerId = "2005", FirstName = "Preston", LastName = "Pritt", BirthDate = new DateOnly(1972, 6, 9), JoinDate = new DateOnly(2013, 2, 2) }
        , new EmployeeDTO() { Id = "5049", Active = true, CountryId = "us", FirstName = "Coleman", LastName = "Bravo", BirthDate = new DateOnly(1981, 6, 2), JoinDate = new DateOnly(2006, 3, 1) }
        , new EmployeeDTO() { Id = "5052", Active = true, CountryId = "ca", FirstName = "Balduino", LastName = "Lee", BirthDate = new DateOnly(1973, 11, 21), JoinDate = new DateOnly(2010, 9, 23) }
        , new EmployeeDTO() { Id = "5053", Active = true, CountryId = "es", ManagerId = "2011", FirstName = "Edric", LastName = "Hoit" }
        , new EmployeeDTO() { Id = "5055", Active = true, CountryId = "usx", ManagerId = "2008", FirstName = "Balduino", LastName = "Battey" }
        /*750*/
        , new EmployeeDTO() { Id = "5056", CountryId = "us", ManagerId = "2010", FirstName = "Rose", LastName = "Ponte", BirthDate = new DateOnly(1962, 2, 14) }
        , new EmployeeDTO() { Id = "5058", Active = true, CountryId = "es", ManagerId = "1002", FirstName = "Elron", LastName = "Taylor", BirthDate = new DateOnly(1978, 7, 3), JoinDate = new DateOnly(2013, 2, 9) }
        , new EmployeeDTO() { Id = "5059", Active = true, CountryId = "ae", FirstName = "Elias", LastName = "Diaz", BirthDate = new DateOnly(1962, 9, 16) }
        , new EmployeeDTO() { Id = "5060", Active = true, CountryId = "ae", ManagerId = "2003", FirstName = "Sydney", LastName = "Platten", BirthDate = new DateOnly(1973, 4, 22), JoinDate = new DateOnly(2010, 11, 26) }
        , new EmployeeDTO() { Id = "5061", Active = true, CountryId = "es", ManagerId = "1001", FirstName = "Advikha", LastName = "Hammer", BirthDate = new DateOnly(1979, 9, 5) }
        /*755*/
        , new EmployeeDTO() { Id = "5064", Active = true, CountryId = "usx", FirstName = "Bedford", LastName = "Sundaram", BirthDate = new DateOnly(1965, 5, 4), JoinDate = new DateOnly(2016, 8, 21) }
        , new EmployeeDTO() { Id = "5065", Active = true, CountryId = "ae", ManagerId = "2002", FirstName = "Eduardo", LastName = "Silva", BirthDate = new DateOnly(1982, 6, 2), JoinDate = new DateOnly(2013, 3, 26), StartTime = new TimeOnly(1, 33, 12) }
        , new EmployeeDTO() { Id = "5069", Active = true, CountryId = "us", FirstName = "Louise", LastName = "Keller", BirthDate = new DateOnly(1959, 4, 27), JoinDate = new DateOnly(2014, 2, 12) }
        , new EmployeeDTO() { Id = "5071", Active = true, CountryId = "usx", ManagerId = "2008", FirstName = "Preston", LastName = "Varma", BirthDate = new DateOnly(1963, 2, 25), JoinDate = new DateOnly(2011, 3, 9), StartTime = new TimeOnly(7, 1, 14) }
        , new EmployeeDTO() { Id = "5073", Active = true, CountryId = "usx", ManagerId = "1001", FirstName = "Ernesto", LastName = "Feimster", BirthDate = new DateOnly(1962, 1, 18) }
        /*760*/
        , new EmployeeDTO() { Id = "5075", Active = true, CountryId = "it", ManagerId = "2011", FirstName = "Andrea", LastName = "Bravo", BirthDate = new DateOnly(1978, 11, 6), JoinDate = new DateOnly(2005, 8, 14) }
        , new EmployeeDTO() { Id = "5076", CountryId = "ae", ManagerId = "2002", FirstName = "Wilbur", LastName = "Saxe", BirthDate = new DateOnly(1975, 9, 12), JoinDate = new DateOnly(2006, 3, 14), StartTime = new TimeOnly(4, 8, 6) }
        , new EmployeeDTO() { Id = "5077", Active = true, CountryId = "ae", FirstName = "Coleman", LastName = "Segarra" }
        , new EmployeeDTO() { Id = "5080", CountryId = "ie", FirstName = "Blythe", LastName = "Garland" }
        , new EmployeeDTO() { Id = "5082", Active = true, CountryId = "it", ManagerId = "2006", FirstName = "Carter", LastName = "Baver", BirthDate = new DateOnly(1976, 5, 24), JoinDate = new DateOnly(2009, 3, 24) }
        /*765*/
        , new EmployeeDTO() { Id = "5083", Active = true, CountryId = "it", FirstName = "Sydney", LastName = "Brown", BirthDate = new DateOnly(1966, 5, 4) }
        , new EmployeeDTO() { Id = "5084", Active = true, CountryId = "us", ManagerId = "1002", FirstName = "Elena", LastName = "Mitchum", BirthDate = new DateOnly(1980, 3, 23), JoinDate = new DateOnly(2011, 4, 4), StartTime = new TimeOnly(13, 44, 22) }
        , new EmployeeDTO() { Id = "5085", Active = true, CountryId = "ae", ManagerId = "2008", FirstName = "Elvira", LastName = "Quiroga", BirthDate = new DateOnly(1976, 2, 10), JoinDate = new DateOnly(2011, 10, 27), StartTime = new TimeOnly(19, 18, 17) }
        , new EmployeeDTO() { Id = "5086", Active = true, CountryId = "ae", ManagerId = "2008", FirstName = "Navya", LastName = "Kiana", BirthDate = new DateOnly(1978, 3, 2), JoinDate = new DateOnly(2005, 1, 2), StartTime = new TimeOnly(18, 8, 42) }
        , new EmployeeDTO() { Id = "5087", Active = true, CountryId = "pt", ManagerId = "2010", FirstName = "Norton", LastName = "Batte", BirthDate = new DateOnly(1976, 4, 4), JoinDate = new DateOnly(2008, 6, 7), StartTime = new TimeOnly(9, 12, 46) }
        /*770*/
        , new EmployeeDTO() { Id = "5088", Active = true, CountryId = "pt", ManagerId = "2008", FirstName = "Javier", LastName = "Mitchum", BirthDate = new DateOnly(1971, 9, 24), JoinDate = new DateOnly(2013, 7, 10), StartTime = new TimeOnly(14, 41, 39) }
        , new EmployeeDTO() { Id = "5089", Active = true, CountryId = "mx", ManagerId = "2010", FirstName = "Edric", LastName = "Fenning", BirthDate = new DateOnly(1979, 10, 18) }
        , new EmployeeDTO() { Id = "5090", Active = true, CountryId = "ie", ManagerId = "2010", FirstName = "Emilio", LastName = "Beach" }
        , new EmployeeDTO() { Id = "5091", Active = true, CountryId = "ae", ManagerId = "2010", FirstName = "Clara", LastName = "Godoy", BirthDate = new DateOnly(1971, 8, 15) }
        , new EmployeeDTO() { Id = "5095", Active = true, CountryId = "it", ManagerId = "2005", FirstName = "Bedford", LastName = "Peiro" }
        /*775*/
        , new EmployeeDTO() { Id = "5096", Active = true, CountryId = "pt", FirstName = "Edney", LastName = "Oliva", BirthDate = new DateOnly(1971, 8, 20) }
        , new EmployeeDTO() { Id = "5099", Active = true, CountryId = "uk", FirstName = "Charvi", LastName = "Heavens", BirthDate = new DateOnly(1966, 10, 10), JoinDate = new DateOnly(2010, 4, 5), StartTime = new TimeOnly(11, 43, 30) }
        , new EmployeeDTO() { Id = "5100", Active = true, CountryId = "ae", ManagerId = "2005", FirstName = "Rudolph", LastName = "Faulkner", BirthDate = new DateOnly(1977, 1, 19), JoinDate = new DateOnly(2008, 4, 17), StartTime = new TimeOnly(20, 39, 11) }
        , new EmployeeDTO() { Id = "5101", Active = true, CountryId = "pt", ManagerId = "2008", FirstName = "Samar", LastName = "Sherwood", BirthDate = new DateOnly(1976, 10, 13), JoinDate = new DateOnly(2005, 4, 2), StartTime = new TimeOnly(6, 5, 18) }
        , new EmployeeDTO() { Id = "5102", CountryId = "ie", ManagerId = "2002", FirstName = "Bedford", LastName = "Layton", BirthDate = new DateOnly(1965, 10, 3), JoinDate = new DateOnly(2009, 4, 15) }
        /*780*/
        , new EmployeeDTO() { Id = "5105", Active = true, CountryId = "usx", ManagerId = "2001", FirstName = "Preston", LastName = "Kenerly", BirthDate = new DateOnly(1972, 6, 26), JoinDate = new DateOnly(2008, 11, 21), StartTime = new TimeOnly(6, 25, 46) }
        , new EmployeeDTO() { Id = "5111", Active = true, CountryId = "usx", FirstName = "Rita", LastName = "Tyreck", BirthDate = new DateOnly(1966, 3, 20), JoinDate = new DateOnly(2014, 7, 1) }
        , new EmployeeDTO() { Id = "5113", Active = true, CountryId = "za", ManagerId = "2006", FirstName = "Eduardo", LastName = "Bala" }
        , new EmployeeDTO() { Id = "5115", Active = true, CountryId = "za", ManagerId = "2002", FirstName = "Edric", LastName = "Robards", BirthDate = new DateOnly(1964, 3, 13) }
        , new EmployeeDTO() { Id = "5116", CountryId = "it", ManagerId = "1001", FirstName = "Eduardo", LastName = "Wells", BirthDate = new DateOnly(1979, 8, 18) }
        /*785*/
        , new EmployeeDTO() { Id = "5118", Active = true, CountryId = "usx", ManagerId = "1002", FirstName = "Norton", LastName = "Paxton", BirthDate = new DateOnly(1959, 7, 7), JoinDate = new DateOnly(2010, 10, 17) }
        , new EmployeeDTO() { Id = "5119", Active = true, CountryId = "es", ManagerId = "1001", FirstName = "Brandon", LastName = "Ranger", BirthDate = new DateOnly(1981, 7, 8), JoinDate = new DateOnly(2011, 1, 27) }
        , new EmployeeDTO() { Id = "5120", Active = true, CountryId = "it", ManagerId = "1002", FirstName = "Samar", LastName = "Mitchum", BirthDate = new DateOnly(1964, 11, 12), JoinDate = new DateOnly(2012, 8, 17), StartTime = new TimeOnly(0, 14, 22) }
        , new EmployeeDTO() { Id = "5121", Active = true, CountryId = "ae", ManagerId = "1002", FirstName = "Brandon", LastName = "Heavens", BirthDate = new DateOnly(1960, 11, 12), JoinDate = new DateOnly(2007, 7, 24), StartTime = new TimeOnly(2, 2, 20) }
        , new EmployeeDTO() { Id = "5122", Active = true, CountryId = "uk", ManagerId = "1001", FirstName = "Eugenio", LastName = "Mitchum", BirthDate = new DateOnly(1964, 11, 3), JoinDate = new DateOnly(2009, 8, 12), StartTime = new TimeOnly(22, 36, 37) }
        /*790*/
        , new EmployeeDTO() { Id = "5124", Active = true, CountryId = "us", FirstName = "Louise", LastName = "Hudson", BirthDate = new DateOnly(1979, 6, 3) }
        , new EmployeeDTO() { Id = "5125", Active = true, CountryId = "ie", ManagerId = "2002", FirstName = "Elena", LastName = "Battey", BirthDate = new DateOnly(1979, 1, 7) }
        , new EmployeeDTO() { Id = "5127", CountryId = "mx", FirstName = "Rose", LastName = "Black", BirthDate = new DateOnly(1981, 10, 26), JoinDate = new DateOnly(2007, 6, 9) }
        , new EmployeeDTO() { Id = "5128", Active = true, CountryId = "za", ManagerId = "2003", FirstName = "John", LastName = "Groove", BirthDate = new DateOnly(1978, 9, 14), JoinDate = new DateOnly(2008, 8, 2), StartTime = new TimeOnly(14, 30, 26) }
        , new EmployeeDTO() { Id = "5129", CountryId = "ca", ManagerId = "2006", FirstName = "Ainsley", LastName = "Gutierrez", BirthDate = new DateOnly(1960, 2, 2) }
        /*795*/
        , new EmployeeDTO() { Id = "5130", Active = true, CountryId = "it", FirstName = "Samar", LastName = "Bayley", BirthDate = new DateOnly(1969, 6, 3) }
        , new EmployeeDTO() { Id = "5132", Active = true, CountryId = "it", ManagerId = "2010", FirstName = "Javier", LastName = "Hurley", BirthDate = new DateOnly(1962, 6, 12), JoinDate = new DateOnly(2014, 5, 17), StartTime = new TimeOnly(15, 43, 50) }
        , new EmployeeDTO() { Id = "5133", Active = true, CountryId = "ie", ManagerId = "2006", FirstName = "Brandon", LastName = "Fleet", BirthDate = new DateOnly(1970, 1, 5) }
        , new EmployeeDTO() { Id = "5134", CountryId = "ae", ManagerId = "2008", FirstName = "Sahil", LastName = "Richarson", BirthDate = new DateOnly(1959, 7, 23), JoinDate = new DateOnly(2009, 8, 14), StartTime = new TimeOnly(0, 1, 51) }
        , new EmployeeDTO() { Id = "5135", Active = true, CountryId = "ie", ManagerId = "2002", FirstName = "Antonio", LastName = "Jover", BirthDate = new DateOnly(1965, 10, 2) }
        /*800*/
        , new EmployeeDTO() { Id = "5136", Active = true, CountryId = "uk", ManagerId = "2006", FirstName = "Edison", LastName = "Pinson", BirthDate = new DateOnly(1968, 4, 20), JoinDate = new DateOnly(2015, 5, 7), StartTime = new TimeOnly(15, 35, 24) }
        , new EmployeeDTO() { Id = "5137", CountryId = "uk", FirstName = "Ira", LastName = "Adams", BirthDate = new DateOnly(1964, 10, 10), JoinDate = new DateOnly(2010, 10, 11) }
        , new EmployeeDTO() { Id = "5138", Active = true, CountryId = "usx", FirstName = "Wilfred", LastName = "Marrioter", BirthDate = new DateOnly(1970, 6, 23) }
        , new EmployeeDTO() { Id = "5139", Active = true, CountryId = "ca", ManagerId = "2006", FirstName = "Emilio", LastName = "Wells", BirthDate = new DateOnly(1972, 3, 6), JoinDate = new DateOnly(2006, 5, 26) }
        , new EmployeeDTO() { Id = "5142", Active = true, CountryId = "ca", ManagerId = "2001", FirstName = "Sydney", LastName = "Hampston", BirthDate = new DateOnly(1961, 5, 13) }
        /*805*/
        , new EmployeeDTO() { Id = "5143", Active = true, CountryId = "usx", ManagerId = "2010", FirstName = "Edison", LastName = "Wayman", BirthDate = new DateOnly(1964, 6, 20), JoinDate = new DateOnly(2009, 1, 20), StartTime = new TimeOnly(12, 52, 28) }
        , new EmployeeDTO() { Id = "5144", CountryId = "ie", FirstName = "Enrique", LastName = "Batton", BirthDate = new DateOnly(1962, 10, 19), JoinDate = new DateOnly(2008, 1, 12) }
        , new EmployeeDTO() { Id = "5146", Active = true, CountryId = "it", ManagerId = "1002", FirstName = "Norton", LastName = "Lawes", BirthDate = new DateOnly(1980, 1, 19), JoinDate = new DateOnly(2006, 5, 27), StartTime = new TimeOnly(22, 36, 19) }
        , new EmployeeDTO() { Id = "5148", CountryId = "ca", FirstName = "Prynash", LastName = "Seton", BirthDate = new DateOnly(1962, 5, 15) }
        , new EmployeeDTO() { Id = "5149", Active = true, CountryId = "pt", ManagerId = "2011", FirstName = "Coleman", LastName = "Chesnut" }
        /*810*/
        , new EmployeeDTO() { Id = "5150", Active = true, CountryId = "za", ManagerId = "2001", FirstName = "Wilfred", LastName = "Tinsleay", BirthDate = new DateOnly(1963, 4, 12), JoinDate = new DateOnly(2011, 7, 15) }
        , new EmployeeDTO() { Id = "5151", Active = true, CountryId = "mx", ManagerId = "2006", FirstName = "Navya", LastName = "Yates", BirthDate = new DateOnly(1973, 3, 3) }
        , new EmployeeDTO() { Id = "5152", Active = true, CountryId = "za", ManagerId = "2005", FirstName = "Norton", LastName = "Faulkner", BirthDate = new DateOnly(1968, 4, 26) }
        , new EmployeeDTO() { Id = "5155", CountryId = "uk", ManagerId = "2011", FirstName = "Elron", LastName = "Black", BirthDate = new DateOnly(1982, 3, 19), JoinDate = new DateOnly(2009, 8, 5) }
        , new EmployeeDTO() { Id = "5156", CountryId = "pt", ManagerId = "1001", FirstName = "Ainsley", LastName = "Black", BirthDate = new DateOnly(1963, 8, 18), JoinDate = new DateOnly(2015, 6, 13) }
        /*815*/
        , new EmployeeDTO() { Id = "5158", CountryId = "ie", ManagerId = "2011", FirstName = "Samar", LastName = "Saxe", BirthDate = new DateOnly(1978, 11, 11), JoinDate = new DateOnly(2008, 6, 2) }
        , new EmployeeDTO() { Id = "5159", Active = true, CountryId = "ae", ManagerId = "2003", FirstName = "Diego", LastName = "Batt", BirthDate = new DateOnly(1968, 3, 3), JoinDate = new DateOnly(2010, 7, 11) }
        , new EmployeeDTO() { Id = "5161", Active = true, CountryId = "ca", ManagerId = "1001", FirstName = "Wilfred", LastName = "Sise", BirthDate = new DateOnly(1966, 6, 3), JoinDate = new DateOnly(2016, 3, 16), StartTime = new TimeOnly(3, 0, 49) }
        , new EmployeeDTO() { Id = "5163", Active = true, CountryId = "usx", FirstName = "Advikha", LastName = "Holdker", BirthDate = new DateOnly(1963, 7, 17), JoinDate = new DateOnly(2014, 5, 21) }
        , new EmployeeDTO() { Id = "5164", Active = true, CountryId = "za", ManagerId = "2011", FirstName = "Edison", LastName = "Bravo", BirthDate = new DateOnly(1960, 4, 23), JoinDate = new DateOnly(2008, 5, 11) }
        /*820*/
        , new EmployeeDTO() { Id = "5166", Active = true, CountryId = "es", FirstName = "Rita", LastName = "Dismore", BirthDate = new DateOnly(1982, 2, 25), JoinDate = new DateOnly(2008, 2, 23) }
        , new EmployeeDTO() { Id = "5167", Active = true, CountryId = "pt", ManagerId = "2006", FirstName = "Samaira", LastName = "Cornwell", BirthDate = new DateOnly(1961, 3, 11), JoinDate = new DateOnly(2009, 9, 7) }
        , new EmployeeDTO() { Id = "5168", Active = true, CountryId = "usx", ManagerId = "2011", FirstName = "David", LastName = "Pulhart", BirthDate = new DateOnly(1980, 8, 5) }
        , new EmployeeDTO() { Id = "5170", Active = true, CountryId = "za", FirstName = "Pricilla", LastName = "Eastham" }
        , new EmployeeDTO() { Id = "5171", Active = true, CountryId = "it", ManagerId = "2001", FirstName = "Samar", LastName = "Ponte", BirthDate = new DateOnly(1969, 8, 22) }
        /*825*/
        , new EmployeeDTO() { Id = "5172", Active = true, CountryId = "pt", FirstName = "Mitul", LastName = "Predmore", BirthDate = new DateOnly(1978, 6, 18), JoinDate = new DateOnly(2006, 5, 15), StartTime = new TimeOnly(17, 22, 54) }
        , new EmployeeDTO() { Id = "5173", Active = true, CountryId = "ca", FirstName = "Norton", LastName = "Atcher", BirthDate = new DateOnly(1965, 3, 14) }
        , new EmployeeDTO() { Id = "5174", Active = true, CountryId = "usx", ManagerId = "2011", FirstName = "Prynash", LastName = "Newman", BirthDate = new DateOnly(1960, 8, 21), JoinDate = new DateOnly(2011, 6, 4) }
        , new EmployeeDTO() { Id = "5175", Active = true, CountryId = "ae", ManagerId = "2003", FirstName = "Advikha", LastName = "Zola", BirthDate = new DateOnly(1963, 8, 19) }
        , new EmployeeDTO() { Id = "5176", Active = true, CountryId = "pt", ManagerId = "2003", FirstName = "Ira", LastName = "Keller", BirthDate = new DateOnly(1975, 5, 1), JoinDate = new DateOnly(2010, 6, 4), StartTime = new TimeOnly(11, 28, 40) }
        /*830*/
        , new EmployeeDTO() { Id = "5177", Active = true, CountryId = "es", ManagerId = "2001", FirstName = "Jeremias", LastName = "Bronson", BirthDate = new DateOnly(1961, 10, 16), JoinDate = new DateOnly(2012, 9, 27), StartTime = new TimeOnly(6, 15, 50) }
        , new EmployeeDTO() { Id = "5179", Active = true, CountryId = "us", ManagerId = "2002", FirstName = "Samaira", LastName = "Pinson", BirthDate = new DateOnly(1968, 2, 19), JoinDate = new DateOnly(2005, 1, 17) }
        , new EmployeeDTO() { Id = "5180", Active = true, CountryId = "pt", FirstName = "Coleman", LastName = "Batt", BirthDate = new DateOnly(1980, 5, 20), JoinDate = new DateOnly(2009, 10, 22) }
        , new EmployeeDTO() { Id = "5181", Active = true, CountryId = "ie", ManagerId = "2005", FirstName = "Tina", LastName = "Chesnut", BirthDate = new DateOnly(1978, 9, 19) }
        , new EmployeeDTO() { Id = "5182", Active = true, CountryId = "pt", ManagerId = "2005", FirstName = "Zara", LastName = "Hurley", BirthDate = new DateOnly(1981, 5, 9), JoinDate = new DateOnly(2009, 10, 19) }
        /*835*/
        , new EmployeeDTO() { Id = "5183", Active = true, CountryId = "ae", ManagerId = "1002", FirstName = "Rose", LastName = "Gutierrez" }
        , new EmployeeDTO() { Id = "5184", CountryId = "usx", FirstName = "Tina", LastName = "Machado", BirthDate = new DateOnly(1978, 1, 3) }
        , new EmployeeDTO() { Id = "5185", CountryId = "ca", ManagerId = "2001", FirstName = "Mitul", LastName = "Camino", BirthDate = new DateOnly(1959, 11, 10), JoinDate = new DateOnly(2008, 4, 24) }
        , new EmployeeDTO() { Id = "5186", Active = true, CountryId = "ae", ManagerId = "2008", FirstName = "Ira", LastName = "Alder", BirthDate = new DateOnly(1969, 5, 10) }
        , new EmployeeDTO() { Id = "5187", Active = true, CountryId = "za", FirstName = "Brandon", LastName = "Germano", BirthDate = new DateOnly(1966, 9, 25), JoinDate = new DateOnly(2015, 10, 15) }
        /*840*/
        , new EmployeeDTO() { Id = "5188", Active = true, CountryId = "ca", ManagerId = "2001", FirstName = "Edric", LastName = "Faulkner", BirthDate = new DateOnly(1976, 10, 1), JoinDate = new DateOnly(2009, 9, 25) }
        , new EmployeeDTO() { Id = "5189", Active = true, CountryId = "ae", ManagerId = "2003", FirstName = "Prynash", LastName = "Hartwell", BirthDate = new DateOnly(1973, 11, 17) }
        , new EmployeeDTO() { Id = "5190", Active = true, CountryId = "ca", ManagerId = "2010", FirstName = "Coleman", LastName = "Infante", BirthDate = new DateOnly(1982, 10, 15), JoinDate = new DateOnly(2008, 2, 18) }
        , new EmployeeDTO() { Id = "5191", CountryId = "us", ManagerId = "2003", FirstName = "Wilbur", LastName = "Atcher", BirthDate = new DateOnly(1981, 5, 9), JoinDate = new DateOnly(2011, 8, 23), StartTime = new TimeOnly(4, 16, 16) }
        , new EmployeeDTO() { Id = "5192", Active = true, CountryId = "usx", FirstName = "Advikha", LastName = "Taplin", BirthDate = new DateOnly(1972, 4, 2), JoinDate = new DateOnly(2012, 8, 25) }
        /*845*/
        , new EmployeeDTO() { Id = "5193", Active = true, CountryId = "es", FirstName = "Amory", LastName = "Redondo", BirthDate = new DateOnly(1969, 2, 9), JoinDate = new DateOnly(2005, 5, 11), StartTime = new TimeOnly(18, 5, 28) }
        , new EmployeeDTO() { Id = "5194", Active = true, CountryId = "usx", FirstName = "Elisa", LastName = "Sinyard", BirthDate = new DateOnly(1968, 2, 12) }
        , new EmployeeDTO() { Id = "5195", Active = true, CountryId = "pt", ManagerId = "2002", FirstName = "Brandon", LastName = "Beach", BirthDate = new DateOnly(1964, 10, 21), JoinDate = new DateOnly(2016, 9, 11), StartTime = new TimeOnly(20, 1, 21) }
        , new EmployeeDTO() { Id = "5196", Active = true, CountryId = "usx", FirstName = "Tobin", LastName = "Bayley", BirthDate = new DateOnly(1973, 7, 26), JoinDate = new DateOnly(2013, 5, 14) }
        , new EmployeeDTO() { Id = "5197", Active = true, CountryId = "uk", ManagerId = "2011", FirstName = "Manuel", LastName = "Vohra" }
        /*850*/
        , new EmployeeDTO() { Id = "5199", CountryId = "pt", ManagerId = "2003", FirstName = "Dana", LastName = "Predmore", BirthDate = new DateOnly(1960, 4, 7) }
        , new EmployeeDTO() { Id = "5200", Active = true, CountryId = "es", FirstName = "Rita", LastName = "Eastham", BirthDate = new DateOnly(1964, 9, 2), JoinDate = new DateOnly(2007, 3, 2) }
        , new EmployeeDTO() { Id = "5201", Active = true, CountryId = "ie", FirstName = "Navya", LastName = "Sutterley", BirthDate = new DateOnly(1982, 7, 15) }
        , new EmployeeDTO() { Id = "5202", Active = true, CountryId = "it", FirstName = "Rudolph", LastName = "Quiroga", BirthDate = new DateOnly(1973, 5, 17), JoinDate = new DateOnly(2011, 8, 1), StartTime = new TimeOnly(19, 30, 5) }
        , new EmployeeDTO() { Id = "5204", Active = true, CountryId = "ca", ManagerId = "2003", FirstName = "Rudolph", LastName = "Baxendale" }
        /*855*/
        , new EmployeeDTO() { Id = "5205", CountryId = "za", ManagerId = "2011", FirstName = "Edison", LastName = "Warden", BirthDate = new DateOnly(1965, 9, 2), JoinDate = new DateOnly(2015, 3, 26) }
        , new EmployeeDTO() { Id = "5207", CountryId = "ae", ManagerId = "2005", FirstName = "Edney", LastName = "Sinyard", BirthDate = new DateOnly(1973, 1, 14) }
        , new EmployeeDTO() { Id = "5208", Active = true, CountryId = "ca", ManagerId = "2010", FirstName = "Prynash", LastName = "Vyass", BirthDate = new DateOnly(1982, 3, 16) }
        , new EmployeeDTO() { Id = "5209", Active = true, CountryId = "mx", ManagerId = "2001", FirstName = "Pricilla", LastName = "Kimber", BirthDate = new DateOnly(1968, 4, 1) }
        , new EmployeeDTO() { Id = "5210", Active = true, CountryId = "us", ManagerId = "2011", FirstName = "Tobin", LastName = "Wayman", BirthDate = new DateOnly(1978, 6, 26) }
        /*860*/
        , new EmployeeDTO() { Id = "5212", Active = true, CountryId = "ca", ManagerId = "2003", FirstName = "Ernesto", LastName = "Hernando", BirthDate = new DateOnly(1971, 6, 6) }
        , new EmployeeDTO() { Id = "5213", Active = true, CountryId = "pt", ManagerId = "2002", FirstName = "Andrea", LastName = "Star", BirthDate = new DateOnly(1971, 7, 14) }
        , new EmployeeDTO() { Id = "5215", Active = true, CountryId = "za", ManagerId = "2008", FirstName = "Rudolph", LastName = "Baxendale", BirthDate = new DateOnly(1961, 1, 4), JoinDate = new DateOnly(2013, 5, 7) }
        , new EmployeeDTO() { Id = "5216", CountryId = "mx", FirstName = "Rudolph", LastName = "Duque", BirthDate = new DateOnly(1968, 5, 7), JoinDate = new DateOnly(2012, 10, 16) }
        , new EmployeeDTO() { Id = "5218", Active = true, CountryId = "ie", ManagerId = "1002", FirstName = "Clara", LastName = "Farham", BirthDate = new DateOnly(1969, 2, 15), JoinDate = new DateOnly(2015, 11, 3) }
        /*865*/
        , new EmployeeDTO() { Id = "5219", Active = true, CountryId = "mx", ManagerId = "2001", FirstName = "Jeremias", LastName = "Varma", BirthDate = new DateOnly(1960, 1, 11), JoinDate = new DateOnly(2010, 2, 15) }
        , new EmployeeDTO() { Id = "5220", Active = true, CountryId = "usx", FirstName = "Rita", LastName = "Hurley", BirthDate = new DateOnly(1981, 7, 10), JoinDate = new DateOnly(2008, 6, 13), StartTime = new TimeOnly(6, 32, 53) }
        , new EmployeeDTO() { Id = "5222", CountryId = "uk", ManagerId = "1001", FirstName = "Pricilla", LastName = "Oliva", BirthDate = new DateOnly(1979, 5, 26) }
        , new EmployeeDTO() { Id = "5223", CountryId = "es", ManagerId = "2005", FirstName = "Coleman", LastName = "Machado", BirthDate = new DateOnly(1969, 9, 14), JoinDate = new DateOnly(2013, 11, 11) }
        , new EmployeeDTO() { Id = "5225", CountryId = "us", ManagerId = "2010", FirstName = "Andrea", LastName = "Bayley", BirthDate = new DateOnly(1965, 2, 3), JoinDate = new DateOnly(2012, 11, 13) }
        /*870*/
        , new EmployeeDTO() { Id = "5226", Active = true, CountryId = "ae", ManagerId = "2011", FirstName = "Andrea", LastName = "Star", BirthDate = new DateOnly(1971, 4, 23), JoinDate = new DateOnly(2013, 1, 13) }
        , new EmployeeDTO() { Id = "5227", Active = true, CountryId = "pt", ManagerId = "2003", FirstName = "Stuart", LastName = "Homes", BirthDate = new DateOnly(1963, 7, 1), JoinDate = new DateOnly(2014, 9, 1) }
        , new EmployeeDTO() { Id = "5228", Active = true, CountryId = "it", ManagerId = "2008", FirstName = "Ernesto", LastName = "Zayden", BirthDate = new DateOnly(1963, 3, 22), JoinDate = new DateOnly(2011, 1, 18) }
        , new EmployeeDTO() { Id = "5229", CountryId = "es", ManagerId = "2006", FirstName = "Eshan", LastName = "Talford", BirthDate = new DateOnly(1980, 7, 26), JoinDate = new DateOnly(2008, 8, 15) }
        , new EmployeeDTO() { Id = "5230", CountryId = "uk", ManagerId = "2003", FirstName = "Elena", LastName = "Seton", BirthDate = new DateOnly(1981, 2, 16), JoinDate = new DateOnly(2014, 7, 16), StartTime = new TimeOnly(1, 10, 20) }
        /*875*/
        , new EmployeeDTO() { Id = "5231", Active = true, CountryId = "es", ManagerId = "2010", FirstName = "Thelma", LastName = "Mitchum", BirthDate = new DateOnly(1961, 6, 14) }
        , new EmployeeDTO() { Id = "5232", Active = true, CountryId = "uk", ManagerId = "2011", FirstName = "Andrea", LastName = "Lee", BirthDate = new DateOnly(1971, 1, 22), JoinDate = new DateOnly(2009, 10, 1) }
        , new EmployeeDTO() { Id = "5234", Active = true, CountryId = "pt", ManagerId = "1001", FirstName = "Balduino", LastName = "Bala", BirthDate = new DateOnly(1978, 5, 8), JoinDate = new DateOnly(2010, 4, 17) }
        , new EmployeeDTO() { Id = "5235", Active = true, CountryId = "pt", FirstName = "Stuart", LastName = "Germano", BirthDate = new DateOnly(1965, 11, 1), JoinDate = new DateOnly(2014, 10, 24) }
        , new EmployeeDTO() { Id = "5236", Active = true, CountryId = "mx", FirstName = "Beverly", LastName = "Pole", BirthDate = new DateOnly(1980, 11, 2), JoinDate = new DateOnly(2012, 6, 14) }
        /*880*/
        , new EmployeeDTO() { Id = "5237", Active = true, CountryId = "mx", ManagerId = "2011", FirstName = "Blythe", LastName = "Quiroga", BirthDate = new DateOnly(1978, 7, 14), JoinDate = new DateOnly(2016, 6, 11), StartTime = new TimeOnly(7, 1, 37) }
        , new EmployeeDTO() { Id = "5239", Active = true, CountryId = "us", ManagerId = "2010", FirstName = "Trenton", LastName = "Taplin", BirthDate = new DateOnly(1975, 6, 14) }
        , new EmployeeDTO() { Id = "5240", Active = true, CountryId = "it", FirstName = "Ira", LastName = "Shanika", BirthDate = new DateOnly(1962, 11, 11), JoinDate = new DateOnly(2015, 5, 18), StartTime = new TimeOnly(6, 26, 51) }
        , new EmployeeDTO() { Id = "5242", Active = true, CountryId = "ca", ManagerId = "1001", FirstName = "Cassandra", LastName = "Segarra", BirthDate = new DateOnly(1961, 2, 1), JoinDate = new DateOnly(2010, 9, 3) }
        , new EmployeeDTO() { Id = "5244", CountryId = "za", FirstName = "Norton", LastName = "Checa", BirthDate = new DateOnly(1974, 4, 1), JoinDate = new DateOnly(2005, 7, 11), StartTime = new TimeOnly(16, 33, 38) }
        /*885*/
        , new EmployeeDTO() { Id = "5245", Active = true, CountryId = "us", ManagerId = "2008", FirstName = "Antonio", LastName = "Mitton", BirthDate = new DateOnly(1974, 5, 16), JoinDate = new DateOnly(2013, 8, 27) }
        , new EmployeeDTO() { Id = "5247", Active = true, CountryId = "ca", ManagerId = "2002", FirstName = "Eduardo", LastName = "Keller", BirthDate = new DateOnly(1978, 2, 11), JoinDate = new DateOnly(2006, 1, 14) }
        , new EmployeeDTO() { Id = "5248", Active = true, CountryId = "pt", FirstName = "Louise", LastName = "Robey", BirthDate = new DateOnly(1971, 5, 1) }
        , new EmployeeDTO() { Id = "5253", Active = true, CountryId = "uk", ManagerId = "2010", FirstName = "Prynash", LastName = "Alder", BirthDate = new DateOnly(1976, 3, 21), JoinDate = new DateOnly(2009, 4, 14), StartTime = new TimeOnly(19, 46, 3) }
        , new EmployeeDTO() { Id = "5254", Active = true, CountryId = "us", ManagerId = "1001", FirstName = "Edric", LastName = "Vohra", BirthDate = new DateOnly(1961, 1, 13), JoinDate = new DateOnly(2005, 4, 5) }
        /*890*/
        , new EmployeeDTO() { Id = "5255", Active = true, CountryId = "es", ManagerId = "1001", FirstName = "Eshan", LastName = "Pinckok", BirthDate = new DateOnly(1964, 5, 3), JoinDate = new DateOnly(2010, 8, 13) }
        , new EmployeeDTO() { Id = "5257", Active = true, CountryId = "uk", ManagerId = "2006", FirstName = "Brandon", LastName = "Machado", BirthDate = new DateOnly(1971, 4, 6), JoinDate = new DateOnly(2012, 8, 7), StartTime = new TimeOnly(0, 10, 27) }
        , new EmployeeDTO() { Id = "5258", Active = true, CountryId = "pt", FirstName = "Elron", LastName = "Redding", BirthDate = new DateOnly(1978, 7, 10), JoinDate = new DateOnly(2008, 2, 9) }
        , new EmployeeDTO() { Id = "5260", Active = true, CountryId = "usx", FirstName = "Thelma", LastName = "Calloway", BirthDate = new DateOnly(1973, 9, 24) }
        , new EmployeeDTO() { Id = "5261", Active = true, CountryId = "es", ManagerId = "1002", FirstName = "Wilfred", LastName = "Mitchum", BirthDate = new DateOnly(1980, 4, 19) }
        /*895*/
        , new EmployeeDTO() { Id = "5262", Active = true, CountryId = "ae", FirstName = "Charvi", LastName = "Wayman", BirthDate = new DateOnly(1983, 10, 20), JoinDate = new DateOnly(2008, 5, 14), StartTime = new TimeOnly(0, 14, 35) }
        , new EmployeeDTO() { Id = "5264", Active = true, CountryId = "za", FirstName = "Ernesto", LastName = "Hankin", BirthDate = new DateOnly(1964, 6, 13), JoinDate = new DateOnly(2010, 8, 25) }
        , new EmployeeDTO() { Id = "5265", Active = true, CountryId = "uk", FirstName = "Edney", LastName = "Batterton", BirthDate = new DateOnly(1983, 4, 19), JoinDate = new DateOnly(2014, 10, 16) }
        , new EmployeeDTO() { Id = "5266", Active = true, CountryId = "us", FirstName = "Antonio", LastName = "Lumbrearas", BirthDate = new DateOnly(1965, 9, 13), JoinDate = new DateOnly(2016, 9, 20) }
        , new EmployeeDTO() { Id = "5270", Active = true, CountryId = "us", ManagerId = "2011", FirstName = "Antonio", LastName = "Homes", BirthDate = new DateOnly(1966, 9, 11) }
        /*900*/
        , new EmployeeDTO() { Id = "5271", Active = true, CountryId = "za", ManagerId = "2011", FirstName = "Eshan", LastName = "Chesnut" }
        , new EmployeeDTO() { Id = "5272", Active = true, CountryId = "usx", ManagerId = "1001", FirstName = "Wilbur", LastName = "Chandra" }
        , new EmployeeDTO() { Id = "5274", Active = true, CountryId = "ie", FirstName = "Elron", LastName = "Millan" }
        , new EmployeeDTO() { Id = "5275", Active = true, CountryId = "mx", ManagerId = "2008", FirstName = "Ira", LastName = "Jones", BirthDate = new DateOnly(1976, 1, 22) }
        , new EmployeeDTO() { Id = "5276", CountryId = "uk", ManagerId = "2002", FirstName = "Pricilla", LastName = "Alcaraz", BirthDate = new DateOnly(1959, 9, 15), JoinDate = new DateOnly(2010, 11, 17), StartTime = new TimeOnly(8, 55, 46) }
        /*905*/
        , new EmployeeDTO() { Id = "5277", Active = true, CountryId = "us", ManagerId = "2010", FirstName = "Navya", LastName = "Harley", BirthDate = new DateOnly(1974, 6, 9), JoinDate = new DateOnly(2007, 2, 26) }
        , new EmployeeDTO() { Id = "5278", Active = true, CountryId = "es", ManagerId = "2003", FirstName = "Pricilla", LastName = "Cullen" }
        , new EmployeeDTO() { Id = "5279", CountryId = "uk", FirstName = "Emilio", LastName = "Kallen", BirthDate = new DateOnly(1984, 11, 14) }
        , new EmployeeDTO() { Id = "5281", Active = true, CountryId = "uk", ManagerId = "2001", FirstName = "Clara", LastName = "Horton", BirthDate = new DateOnly(1966, 6, 7) }
        , new EmployeeDTO() { Id = "5282", Active = true, CountryId = "ie", ManagerId = "1001", FirstName = "Jeremias", LastName = "Lawes", BirthDate = new DateOnly(1974, 5, 7), JoinDate = new DateOnly(2012, 4, 1), StartTime = new TimeOnly(4, 7, 27) }
        /*910*/
        , new EmployeeDTO() { Id = "5283", Active = true, CountryId = "ie", FirstName = "Pricilla", LastName = "Shanika", BirthDate = new DateOnly(1963, 9, 21) }
        , new EmployeeDTO() { Id = "5284", Active = true, CountryId = "uk", ManagerId = "2010", FirstName = "Rudolph", LastName = "Risley", BirthDate = new DateOnly(1962, 10, 19) }
        , new EmployeeDTO() { Id = "5285", CountryId = "ae", ManagerId = "2010", FirstName = "Sydney", LastName = "Infante", BirthDate = new DateOnly(1960, 6, 20), JoinDate = new DateOnly(2009, 10, 18) }
        , new EmployeeDTO() { Id = "5286", CountryId = "it", ManagerId = "2011", FirstName = "Preston", LastName = "Kiana" }
        , new EmployeeDTO() { Id = "5287", Active = true, CountryId = "ca", ManagerId = "2006", FirstName = "Wilbur", LastName = "Baxley", BirthDate = new DateOnly(1979, 5, 16) }
        /*915*/
        , new EmployeeDTO() { Id = "5288", Active = true, CountryId = "uk", ManagerId = "2010", FirstName = "Blythe", LastName = "Amador", BirthDate = new DateOnly(1982, 10, 9) }
        , new EmployeeDTO() { Id = "5289", CountryId = "za", ManagerId = "2006", FirstName = "Edney", LastName = "Risley", BirthDate = new DateOnly(1964, 5, 27), JoinDate = new DateOnly(2015, 9, 22) }
        , new EmployeeDTO() { Id = "5291", Active = true, CountryId = "es", ManagerId = "2001", FirstName = "Advikha", LastName = "Shadow", BirthDate = new DateOnly(1976, 10, 16) }
        , new EmployeeDTO() { Id = "5292", Active = true, CountryId = "usx", FirstName = "Edric", LastName = "Robey", BirthDate = new DateOnly(1979, 1, 19), JoinDate = new DateOnly(2012, 3, 14), StartTime = new TimeOnly(17, 52, 13) }
        , new EmployeeDTO() { Id = "5293", Active = true, CountryId = "ae", ManagerId = "2003", FirstName = "Thelma", LastName = "Christon", BirthDate = new DateOnly(1974, 5, 23), JoinDate = new DateOnly(2011, 5, 16), StartTime = new TimeOnly(17, 47, 14) }
        /*920*/
        , new EmployeeDTO() { Id = "5295", CountryId = "us", ManagerId = "2011", FirstName = "Louise", LastName = "Risley", BirthDate = new DateOnly(1959, 5, 22) }
        , new EmployeeDTO() { Id = "5297", Active = true, CountryId = "it", ManagerId = "2006", FirstName = "Jeremias", LastName = "Linn", BirthDate = new DateOnly(1967, 3, 18) }
        , new EmployeeDTO() { Id = "5298", Active = true, CountryId = "za", ManagerId = "2010", FirstName = "Ernesto", LastName = "Kenerly", BirthDate = new DateOnly(1962, 5, 17), JoinDate = new DateOnly(2016, 9, 24) }
        , new EmployeeDTO() { Id = "5299", Active = true, CountryId = "ie", ManagerId = "2003", FirstName = "Bedford", LastName = "Nakisha", BirthDate = new DateOnly(1975, 11, 9), JoinDate = new DateOnly(2006, 8, 17) }
        , new EmployeeDTO() { Id = "5301", Active = true, CountryId = "uk", ManagerId = "1002", FirstName = "Rita", LastName = "Kaden", BirthDate = new DateOnly(1962, 2, 21), JoinDate = new DateOnly(2012, 4, 12) }
        /*925*/
        , new EmployeeDTO() { Id = "5303", Active = true, CountryId = "uk", ManagerId = "2011", FirstName = "Prynash", LastName = "Platten", BirthDate = new DateOnly(1966, 6, 18), JoinDate = new DateOnly(2013, 1, 19), StartTime = new TimeOnly(10, 30, 13) }
        , new EmployeeDTO() { Id = "5304", CountryId = "usx", ManagerId = "2002", FirstName = "Elvira", LastName = "Thompson", BirthDate = new DateOnly(1982, 1, 16) }
        , new EmployeeDTO() { Id = "5305", Active = true, CountryId = "ie", FirstName = "Louise", LastName = "Batt", BirthDate = new DateOnly(1972, 2, 5) }
        , new EmployeeDTO() { Id = "5307", Active = true, CountryId = "ie", ManagerId = "2003", FirstName = "Thelma", LastName = "Pole", BirthDate = new DateOnly(1975, 3, 9), JoinDate = new DateOnly(2010, 3, 10) }
        , new EmployeeDTO() { Id = "5309", Active = true, CountryId = "us", ManagerId = "2001", FirstName = "Elisa", LastName = "Brookes", BirthDate = new DateOnly(1968, 6, 25), JoinDate = new DateOnly(2006, 9, 21), StartTime = new TimeOnly(13, 2, 53) }
        /*930*/
        , new EmployeeDTO() { Id = "5310", Active = true, CountryId = "pt", FirstName = "Wilbur", LastName = "Graeme", BirthDate = new DateOnly(1960, 3, 13) }
        , new EmployeeDTO() { Id = "5311", Active = true, CountryId = "ie", ManagerId = "2001", FirstName = "Prisha", LastName = "Quinton", BirthDate = new DateOnly(1961, 4, 26), JoinDate = new DateOnly(2016, 3, 15) }
        , new EmployeeDTO() { Id = "5312", Active = true, CountryId = "uk", FirstName = "Balduino", LastName = "Jax", BirthDate = new DateOnly(1975, 4, 27) }
        , new EmployeeDTO() { Id = "5313", CountryId = "ae", ManagerId = "2010", FirstName = "Preston", LastName = "Aswell", BirthDate = new DateOnly(1967, 3, 6), JoinDate = new DateOnly(2011, 9, 3) }
        , new EmployeeDTO() { Id = "5314", Active = true, CountryId = "it", FirstName = "Charvi", LastName = "Jones", BirthDate = new DateOnly(1980, 5, 27) }
        /*935*/
        , new EmployeeDTO() { Id = "5316", Active = true, CountryId = "ie", ManagerId = "2005", FirstName = "David", LastName = "Varma", BirthDate = new DateOnly(1969, 11, 22), JoinDate = new DateOnly(2013, 8, 2) }
        , new EmployeeDTO() { Id = "5317", Active = true, CountryId = "pt", FirstName = "Mitul", LastName = "Jover", BirthDate = new DateOnly(1974, 4, 8) }
        , new EmployeeDTO() { Id = "5318", CountryId = "pt", ManagerId = "1002", FirstName = "Elias", LastName = "Fuster", BirthDate = new DateOnly(1977, 9, 22) }
        , new EmployeeDTO() { Id = "5319", Active = true, CountryId = "pt", FirstName = "Rudolph", LastName = "Quant", BirthDate = new DateOnly(1972, 4, 21) }
        , new EmployeeDTO() { Id = "5320", Active = true, CountryId = "it", ManagerId = "2010", FirstName = "Advikha", LastName = "Worley" }
        /*940*/
        , new EmployeeDTO() { Id = "5321", Active = true, CountryId = "es", FirstName = "Edison", LastName = "Purrington", BirthDate = new DateOnly(1977, 11, 1), JoinDate = new DateOnly(2011, 2, 21) }
        , new EmployeeDTO() { Id = "5322", Active = true, CountryId = "usx", ManagerId = "2008", FirstName = "Elena", LastName = "Heiden", BirthDate = new DateOnly(1970, 9, 23) }
        , new EmployeeDTO() { Id = "5324", Active = true, CountryId = "ca", FirstName = "Beverly", LastName = "Graeme", BirthDate = new DateOnly(1963, 4, 21) }
        , new EmployeeDTO() { Id = "5325", Active = true, CountryId = "usx", ManagerId = "2001", FirstName = "Brandon", LastName = "Nordan", BirthDate = new DateOnly(1970, 7, 23), JoinDate = new DateOnly(2010, 3, 6), StartTime = new TimeOnly(10, 58, 5) }
        , new EmployeeDTO() { Id = "5326", Active = true, CountryId = "ae", FirstName = "Elvira", LastName = "Hernando", BirthDate = new DateOnly(1967, 4, 6) }
        /*945*/
        , new EmployeeDTO() { Id = "5327", CountryId = "mx", FirstName = "Eduardo", LastName = "Lowden", BirthDate = new DateOnly(1963, 1, 25), JoinDate = new DateOnly(2012, 7, 4) }
        , new EmployeeDTO() { Id = "5328", Active = true, CountryId = "usx", ManagerId = "2001", FirstName = "Ernesto", LastName = "Ponte", BirthDate = new DateOnly(1970, 1, 17) }
        , new EmployeeDTO() { Id = "5329", CountryId = "us", FirstName = "Wilbur", LastName = "Ponte", BirthDate = new DateOnly(1983, 4, 1), JoinDate = new DateOnly(2009, 11, 23) }
        , new EmployeeDTO() { Id = "5332", Active = true, CountryId = "za", ManagerId = "2002", FirstName = "Diego", LastName = "Aswell", BirthDate = new DateOnly(1970, 9, 23) }
        , new EmployeeDTO() { Id = "5333", Active = true, CountryId = "ie", ManagerId = "1001", FirstName = "Preston", LastName = "Hammer", BirthDate = new DateOnly(1965, 10, 23), JoinDate = new DateOnly(2005, 1, 22), StartTime = new TimeOnly(1, 17, 9) }
        /*950*/
        , new EmployeeDTO() { Id = "5334", CountryId = "ie", FirstName = "Beverly", LastName = "Baylor", BirthDate = new DateOnly(1965, 4, 13) }
        , new EmployeeDTO() { Id = "5335", Active = true, CountryId = "ae", ManagerId = "2006", FirstName = "Ira", LastName = "Broomell", BirthDate = new DateOnly(1969, 8, 22) }
        , new EmployeeDTO() { Id = "5338", Active = true, CountryId = "uk", FirstName = "Cassandra", LastName = "Esparza", BirthDate = new DateOnly(1969, 1, 13) }
        , new EmployeeDTO() { Id = "5339", Active = true, CountryId = "it", ManagerId = "1001", FirstName = "Coleman", LastName = "Feimster", BirthDate = new DateOnly(1965, 11, 19) }
        , new EmployeeDTO() { Id = "5340", Active = true, CountryId = "pt", ManagerId = "2001", FirstName = "Dana", LastName = "Batton", BirthDate = new DateOnly(1971, 2, 13), JoinDate = new DateOnly(2006, 4, 11), StartTime = new TimeOnly(8, 20, 48) }
        /*955*/
        , new EmployeeDTO() { Id = "5342", CountryId = "uk", ManagerId = "1001", FirstName = "Clara", LastName = "Brown", BirthDate = new DateOnly(1974, 10, 24) }
        , new EmployeeDTO() { Id = "5343", CountryId = "pt", ManagerId = "2008", FirstName = "Eshan", LastName = "Kiana", BirthDate = new DateOnly(1960, 11, 15), JoinDate = new DateOnly(2007, 8, 19), StartTime = new TimeOnly(11, 12, 52) }
        , new EmployeeDTO() { Id = "5345", CountryId = "ie", ManagerId = "2002", FirstName = "Elisa", LastName = "Paceiro", BirthDate = new DateOnly(1978, 10, 8), JoinDate = new DateOnly(2009, 5, 13) }
        , new EmployeeDTO() { Id = "5347", Active = true, CountryId = "uk", ManagerId = "2003", FirstName = "Prynash", LastName = "Eastham", BirthDate = new DateOnly(1967, 1, 4), JoinDate = new DateOnly(2009, 11, 21), StartTime = new TimeOnly(10, 30, 6) }
        , new EmployeeDTO() { Id = "5350", Active = true, CountryId = "ca", ManagerId = "2002", FirstName = "Amory", LastName = "Heiden", BirthDate = new DateOnly(1965, 11, 26), JoinDate = new DateOnly(2014, 7, 4) }
        /*960*/
        , new EmployeeDTO() { Id = "5351", CountryId = "ae", ManagerId = "1001", FirstName = "David", LastName = "Graeme", BirthDate = new DateOnly(1967, 9, 7), JoinDate = new DateOnly(2016, 4, 11) }
        , new EmployeeDTO() { Id = "5353", Active = true, CountryId = "mx", ManagerId = "1001", FirstName = "Enrique", LastName = "Veenesh", BirthDate = new DateOnly(1979, 3, 17), JoinDate = new DateOnly(2006, 2, 1) }
        , new EmployeeDTO() { Id = "5354", Active = true, CountryId = "mx", ManagerId = "2002", FirstName = "Brandon", LastName = "Esparza", BirthDate = new DateOnly(1982, 9, 16), JoinDate = new DateOnly(2008, 7, 15) }
        , new EmployeeDTO() { Id = "5355", Active = true, CountryId = "es", ManagerId = "2011", FirstName = "Edney", LastName = "Atkeson", BirthDate = new DateOnly(1977, 3, 23), JoinDate = new DateOnly(2010, 3, 17), StartTime = new TimeOnly(7, 27, 46) }
        , new EmployeeDTO() { Id = "5356", Active = true, CountryId = "ca", ManagerId = "1001", FirstName = "Edric", LastName = "Farham", BirthDate = new DateOnly(1981, 10, 14), JoinDate = new DateOnly(2005, 5, 9) }
        /*965*/
        , new EmployeeDTO() { Id = "5357", Active = true, CountryId = "it", ManagerId = "2010", FirstName = "Balduino", LastName = "Paceiro", BirthDate = new DateOnly(1972, 1, 19), JoinDate = new DateOnly(2009, 10, 23) }
        , new EmployeeDTO() { Id = "5359", Active = true, CountryId = "es", FirstName = "Samar", LastName = "Jover", BirthDate = new DateOnly(1980, 8, 7), JoinDate = new DateOnly(2005, 10, 21) }
        , new EmployeeDTO() { Id = "5360", Active = true, CountryId = "it", ManagerId = "1002", FirstName = "Wilbur", LastName = "Faulkner", BirthDate = new DateOnly(1977, 6, 10), JoinDate = new DateOnly(2005, 8, 21) }
        , new EmployeeDTO() { Id = "5361", Active = true, CountryId = "ie", FirstName = "Cassandra", LastName = "Bronson" }
        , new EmployeeDTO() { Id = "5362", Active = true, CountryId = "za", FirstName = "Edric", LastName = "Milton", BirthDate = new DateOnly(1974, 2, 2), JoinDate = new DateOnly(2010, 1, 10), StartTime = new TimeOnly(0, 14, 49) }
        /*970*/
        , new EmployeeDTO() { Id = "5364", Active = true, CountryId = "ie", FirstName = "Tobin", LastName = "Dismore", BirthDate = new DateOnly(1973, 10, 25), JoinDate = new DateOnly(2011, 9, 10) }
        , new EmployeeDTO() { Id = "5365", Active = true, CountryId = "ca", ManagerId = "2011", FirstName = "Zara", LastName = "Vohra", BirthDate = new DateOnly(1965, 8, 4), JoinDate = new DateOnly(2014, 5, 16) }
        , new EmployeeDTO() { Id = "5366", CountryId = "us", ManagerId = "1002", FirstName = "Antonio", LastName = "Mayoral", BirthDate = new DateOnly(1961, 2, 21), JoinDate = new DateOnly(2005, 8, 19) }
        , new EmployeeDTO() { Id = "5367", Active = true, CountryId = "ie", ManagerId = "1002", FirstName = "Dana", LastName = "Veenesh", BirthDate = new DateOnly(1972, 9, 20), JoinDate = new DateOnly(2012, 1, 21) }
        , new EmployeeDTO() { Id = "5368", Active = true, CountryId = "uk", ManagerId = "2005", FirstName = "Zara", LastName = "Sundaram", BirthDate = new DateOnly(1976, 10, 18) }
        /*975*/
        , new EmployeeDTO() { Id = "5369", CountryId = "ie", ManagerId = "2008", FirstName = "Elena", LastName = "Quinton", BirthDate = new DateOnly(1977, 2, 9), JoinDate = new DateOnly(2013, 4, 8) }
        , new EmployeeDTO() { Id = "5370", Active = true, CountryId = "ae", FirstName = "Antonio", LastName = "Batton", BirthDate = new DateOnly(1967, 3, 26) }
        , new EmployeeDTO() { Id = "5371", Active = true, CountryId = "us", ManagerId = "1002", FirstName = "Elvira", LastName = "Locke", BirthDate = new DateOnly(1973, 8, 13), JoinDate = new DateOnly(2010, 7, 21) }
        , new EmployeeDTO() { Id = "5374", Active = true, CountryId = "ae", ManagerId = "2002", FirstName = "Charvi", LastName = "Teelford", BirthDate = new DateOnly(1972, 4, 8), JoinDate = new DateOnly(2015, 1, 5), StartTime = new TimeOnly(6, 30, 47) }
        , new EmployeeDTO() { Id = "5376", Active = true, CountryId = "ie", ManagerId = "2011", FirstName = "Pricilla", LastName = "Astor", BirthDate = new DateOnly(1970, 10, 9) }
        /*980*/
        , new EmployeeDTO() { Id = "5377", Active = true, CountryId = "ca", FirstName = "Navya", LastName = "Holdker", BirthDate = new DateOnly(1975, 11, 23), JoinDate = new DateOnly(2014, 6, 4), StartTime = new TimeOnly(6, 14, 5) }
        , new EmployeeDTO() { Id = "5379", CountryId = "mx", FirstName = "Rudolph", LastName = "Matel", BirthDate = new DateOnly(1961, 6, 26), JoinDate = new DateOnly(2012, 4, 5) }
        , new EmployeeDTO() { Id = "5380", Active = true, CountryId = "ca", ManagerId = "2008", FirstName = "Rita", LastName = "Zeandre", BirthDate = new DateOnly(1983, 5, 12), JoinDate = new DateOnly(2006, 7, 6) }
        , new EmployeeDTO() { Id = "5381", Active = true, CountryId = "pt", FirstName = "Sydney", LastName = "Sayres", BirthDate = new DateOnly(1973, 6, 24) }
        , new EmployeeDTO() { Id = "5382", Active = true, CountryId = "ca", ManagerId = "2002", FirstName = "Brandon", LastName = "Waylon", BirthDate = new DateOnly(1976, 11, 10), JoinDate = new DateOnly(2012, 2, 12) }
        /*985*/
        , new EmployeeDTO() { Id = "5383", Active = true, CountryId = "ae", ManagerId = "2002", FirstName = "Pricilla", LastName = "Beacham", BirthDate = new DateOnly(1974, 1, 16) }
        , new EmployeeDTO() { Id = "5384", Active = true, CountryId = "us", FirstName = "Advikha", LastName = "White", BirthDate = new DateOnly(1962, 5, 15), JoinDate = new DateOnly(2012, 10, 15), StartTime = new TimeOnly(11, 23, 45) }
        , new EmployeeDTO() { Id = "5385", Active = true, CountryId = "usx", ManagerId = "1001", FirstName = "Sydney", LastName = "Kaden", BirthDate = new DateOnly(1975, 5, 7), JoinDate = new DateOnly(2013, 4, 21), StartTime = new TimeOnly(17, 56, 45) }
        , new EmployeeDTO() { Id = "5386", Active = true, CountryId = "it", ManagerId = "2008", FirstName = "Enrique", LastName = "Hammer", BirthDate = new DateOnly(1959, 2, 15), JoinDate = new DateOnly(2012, 9, 5), StartTime = new TimeOnly(6, 19, 28) }
        , new EmployeeDTO() { Id = "5387", Active = true, CountryId = "us", ManagerId = "2011", FirstName = "Eduardo", LastName = "Wayman", BirthDate = new DateOnly(1968, 1, 15), JoinDate = new DateOnly(2011, 7, 25) }
        /*990*/
        , new EmployeeDTO() { Id = "5389", Active = true, CountryId = "mx", ManagerId = "2002", FirstName = "Enrique", LastName = "Duque", BirthDate = new DateOnly(1976, 4, 16) }
        , new EmployeeDTO() { Id = "5391", CountryId = "ae", FirstName = "Zara", LastName = "Mayoral", BirthDate = new DateOnly(1978, 5, 7) }
        , new EmployeeDTO() { Id = "5392", CountryId = "ae", ManagerId = "2006", FirstName = "Sydney", LastName = "Waylon", BirthDate = new DateOnly(1973, 11, 18) }
        , new EmployeeDTO() { Id = "5394", Active = true, CountryId = "usx", FirstName = "Samaira", LastName = "Batterson", BirthDate = new DateOnly(1960, 10, 22), JoinDate = new DateOnly(2014, 1, 22) }
        , new EmployeeDTO() { Id = "5396", Active = true, CountryId = "ae", FirstName = "Cassandra", LastName = "Tiller" }
        /*995*/
        , new EmployeeDTO() { Id = "5397", Active = true, CountryId = "it", ManagerId = "2001", FirstName = "Enrique", LastName = "Matel", BirthDate = new DateOnly(1977, 11, 17), JoinDate = new DateOnly(2015, 4, 20), StartTime = new TimeOnly(15, 11, 51) }
        , new EmployeeDTO() { Id = "5398", Active = true, CountryId = "uk", ManagerId = "2003", FirstName = "Enrique", LastName = "Baxley", BirthDate = new DateOnly(1967, 4, 13) }
        , new EmployeeDTO() { Id = "5399", Active = true, CountryId = "es", ManagerId = "2005", FirstName = "Samar", LastName = "Bravo", BirthDate = new DateOnly(1980, 11, 20), JoinDate = new DateOnly(2015, 7, 6), StartTime = new TimeOnly(22, 54, 38) }
        , new EmployeeDTO() { Id = "5400", Active = true, CountryId = "us", ManagerId = "2003", FirstName = "Jeremias", LastName = "Richarson", BirthDate = new DateOnly(1973, 10, 15), JoinDate = new DateOnly(2008, 2, 24) }
        , new EmployeeDTO() { Id = "5401", CountryId = "es", ManagerId = "2006", FirstName = "Javier", LastName = "Kenerly", BirthDate = new DateOnly(1973, 7, 21), JoinDate = new DateOnly(2010, 5, 11) }
        /*1000*/
        , new EmployeeDTO() { Id = "6001", Active = true,  CountryId = "it", ManagerId = "5391", FirstName = "Rita", LastName = "Angelini", BirthDate = new DateOnly(1977, 11, 19), JoinDate = new DateOnly(2015, 4, 20), StartTime = new TimeOnly(15, 11, 51) }
        , new EmployeeDTO() { Id = "6002", Active = true,  CountryId = "uk", ManagerId = "5394", FirstName = "Charles", LastName = "Baxton", BirthDate = new DateOnly(1967, 4, 11) }
        , new EmployeeDTO() { Id = "6003", CountryId = "es", ManagerId = "5394", FirstName = "Samuel", LastName = "Bravo", BirthDate = new DateOnly(1980, 11, 2), JoinDate = new DateOnly(2015, 7, 6), StartTime = new TimeOnly(22, 54, 38) }
        , new EmployeeDTO() { Id = "6004", CountryId = "us", ManagerId = "5391", FirstName = "Jeremias", LastName = "Del Monto", BirthDate = new DateOnly(1972, 10, 15), JoinDate = new DateOnly(2008, 2, 24) }
        , new EmployeeDTO() { Id = "6005", CountryId = "es", ManagerId = "5394", FirstName = "Javier", LastName = "Fontanesco", BirthDate = new DateOnly(1971, 7, 21), JoinDate = new DateOnly(2010, 5, 11) }
        /*1005*/
    };
}