var express = require('express');
var app = express();

var sqlite3 = require('sqlite3').verbose();
var db = new sqlite3.Database("mainDB");

const fs = require('fs');
var counter = 0;

const bodyParser = require('body-parser');
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

db.exec("CREATE TABLE IF NOT EXISTS thirdTable(id INTEGER PRIMARY KEY, name TEXT, description TEXT)");
//db.exec("DROP TABLE table4");
db.exec("CREATE TABLE IF NOT EXISTS table4(id INTEGER PRIMARY KEY, tick INTEGER, sendingTime Text, installationSignature TEXT, messageType TEXT, message TEXT)");

app.get('/', function (req, res) {
	res.status(200);
	res.send('alright');
	var reqTime = new Date();
	console.log("Incoming GET request at: " + reqTime.toLocaleString());
	var name = "Req #" + counter;
	++counter;
	var desc = "Time of request is " + reqTime.toLocaleString();
	var statment = "INSERT INTO thirdTable VALUES(NULL, '" + name + "', '" + desc + "')";
	console.log(statment);
	db.exec(statment);
});
app.post('/', function (req, res) {
	console.log(req.body);
	/*
	req.body.message = JSON.parse(req.body.message);
	console.dir(req.body);
	res.status(200);
	res.send('alright');
	var reqTime = new Date();
	console.log("Incoming POST request at: " + reqTime.toLocaleString());
	var name = "Req #" + counter;
	++counter;
	var desc = "Time of request is " + reqTime.toLocaleString();
	var statment = "INSERT INTO table4 VALUES(NULL, " + req.body.tick + ", '" + req.body.sendingTime + "', '" + req.body.installationSignature + "','" + req.body.messageType + "','" + req.body.message + "')";
	//console.log(statment);
	db.exec(statment);
	*/
});

app.use(express.static("C:/xampp/htdocs/gallery/"));
console.log(__dirname);
app.listen(80, function () {
  console.log('The server is running');
});
var timeStart = new Date();

//db.close();

process.on("SIGINT", function(){
	db.close();
	var timeEnd = new Date();
	var log = "[Server time start]: " + timeStart.toLocaleString() + "\n[Server shutdown time]: " + timeEnd.toLocaleString() + "\n[Quantity of request]: " + counter;
	console.log(log);
	fs.writeFile("log file.txt", log);
	process.exit();
});