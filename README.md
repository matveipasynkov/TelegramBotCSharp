# TelegramBotCSharp
This is my seventh project, which was assigned as homework in university. The console application is stored in ConsoleApp, the class library in Library, and the logs are stored in var.
### Main Assignment
### Requirements for the class library
#### 1) The Station class represents the objects described in the CSV file of an individual variant. Fields must be named according to Microsoft naming conventions. Classes must contain constructor(s) to initialize their fields.
#### 2) Non-static CSVProcessing class. Contains methods for reading a csv file and writing to it.
a. Write method: takes as input a collection of objects of type MyType and returns an object of type Stream, which will be used to send the csv document to the
Telegram bot.

b. Read method: takes as input a Stream with a csv file from Telegram-bot and returns a collection of objects of type MyType.
#### 3) Non-static JSONProcessing class. Contains methods for reading a json file and writing to it.
a. Write method: takes as input a collection of objects of type MyType and returns a
an object of type Stream, which will be used to send the json file by Telegram bot.

b. Read method: takes as input a Stream with a json file from Telegram-bot and
returns a collection of objects of type MyType.
#### 4) Class implementations must not violate the Open Close Principle and the Single Responsibility Principle.
#### 5) Type hierarchies shall not violate the Liskov Substitution Principle and shall be designed based on the Dependency Inversion Principle.
#### 6) Class implementations must not violate encapsulation and relationships defined between types, such as providing external references to fields or changing the state of an object without checks.
#### 7) Library classes must be accessible outside the assembly.
#### 8) Each non-static class (if any) must necessarily contain, among others, a parameterless constructor or equivalent descriptions that allow its direct or implicit invocation.
#### 9) It is forbidden to modify the data set for classes that are built on the basis of CSV representations from individual variants (e.g., adding fields not contained in the CSV representation).
#### 10) It is allowed to extend open behavior or add closed functional class members.
#### 11) It is allowed to use own (self-written) class hierarchies in addition to those proposed in the individual variant, also in compliance with OOP principles.
### Telegram bot interface requirements
#### The bot should provide the following functionality:
1. Upload a CSV file for processing, the bot receives the file to be processed by further commands.
further commands.
2. Make a selection by one of the fields of the file. Fields, by which you can make a selection, are specified in the table of the individual variant. To implement this item
it is necessary to use MyType objects and LINQ queries.
3. sort by one of the fields. The fields by which the selection can be sorted are specified in the table of the individual variant. To implement this item it is necessary to
use Station objects and LINQ queries.
4. Download the processed file in CSV or JSON format.
5. Upload the JSON file for processing (your program should be able to accept back the JSON files it created, CSV uploads also apply).
