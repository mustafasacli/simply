# simply
Simply project repository. simply project is used for common db operations.

For sample projects and using u can check out https://github.com/mustafasacli/Simply.TestApps repository.
Firstly you should install mysql database or xamp for portable mysql database. be sure database is running.
go [Simply.TestApps](https://github.com/mustafasacli/Simply.TestApps) repository set subfolder /database/Mysql/.
You should install sql files in order of classicmodels_schema, classicmodels_data and classicmodels_data.
Now, you can connect database. 

In [Simply.TestApps] repository, you can check Simply_Test_Db project. We create a class extended from Simply.Data.Database.SimpleDatabase class.
In this class(SimpleMySqlDatabase) we set connection information.

In SimplyTestConsoleApp project; <br/>
ISimpleDatabase database = new SimpleMySqlDatabase(); // create dtaabse instance
int id = 100;
Any method usages;
- database.Any("select * from customers where customerNumber < @id", new { id });
- database.Any("select * from customers where customerNumber < ?id?", new { id }, <br/>
SimpleCommandSetting.Create(parameterNamePrefix: '?'));
- database.AnyOdbc("select * from customers where customerNumber < ?", new[] { (object)id });
- database.AnyJdbc("select * from customers where customerNumber < ?1", new[] { (object)id });
- SimpleDbCommand simpleDbCommand = new SimpleDbCommand()
            {
                CommandType = CommandType.Text,
                CommandText = "select * from customers where customerNumber < @id"
            };
            simpleDbCommand.AddParameter(new DbCommandParameter { ParameterName = "id", Value = id });
           bool exist = database.Any(simpleDbCommand);
