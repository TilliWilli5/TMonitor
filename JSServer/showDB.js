var express = require('express');
var app = express();

var sqlite3 = require('sqlite3').verbose();
var db = new sqlite3.Database("mainDB");

const fs = require('fs');
//console.log("SELECT * FROM thirdTable LIMIT " + (process.argv[2] || "0") + ", " + (process.argv[3] || "0"));

var statement = "SELECT * FROM table4";
if(process.argv[2])
	statement += " LIMIT " + (process.argv[2] || "0");
if(process.argv[3])
	statement += ", " + (process.argv[3] || "0");
console.log(statement);
db.all(statement, function(pError, pResultSet){
	if(pError)
	{
		console.log("There is some error:\n" + pError);
	}
	else
	{
		//console.dir(pResultSet);
		//console.log("ResultSet:\n" + pResultSet.toString());
		var k=0;
		for(var head in pResultSet[0])
		{
			process.stdout.write(head + "\t");
		}
		process.stdout.write("\n");
		pResultSet.forEach(function(oneRow){
			//console.log("\t[Row#" + k++ + "]");
			for(var prop in oneRow)
			{
				//console.log(prop.toString() + ": " + oneRow[prop]);
				process.stdout.write(oneRow[prop] + "\t");
			}
			process.stdout.write("\n");
		});
	}
});

db.close();
fs.writeFile("showDB_logfile.txt", "The application was closed at " + new Date());