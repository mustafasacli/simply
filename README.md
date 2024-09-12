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
ISimpleDatabase database = new SimpleMySqlDatabase(); // create database instance <br/>

int id = 100;
Any method usages;
- database.Any("select * from customers where customerNumber < @id", new { id }); <br/>
- database.Any("select * from customers where customerNumber < ?id?", new { id }, <br/>
SimpleCommandSetting.Create(parameterNamePrefix: '?')); <br/>
- database.AnyOdbc("select * from customers where customerNumber < ?", new[] { (object)id }); <br/>
- database.AnyJdbc("select * from customers where customerNumber < ?1", new[] { (object)id }); <br/>
- SimpleDbCommand simpleDbCommand = new SimpleDbCommand() <br/>
            { <br/>
                CommandType = CommandType.Text, <br/>
                CommandText = "select * from customers where customerNumber < @id" <br/>
            }; <br/>
            simpleDbCommand.AddParameter(new DbCommandParameter { ParameterName = "id", Value = id }); <br/>
           bool exist = database.Any(simpleDbCommand); <br/>
