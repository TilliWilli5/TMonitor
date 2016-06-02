var express = require('express');
var app = express();

var sqlite3 = require('sqlite3').verbose();
var db = new sqlite3.Database("mainDB");

const fs = require('fs');
var counter = 0;

const bodyParser = require('body-parser');
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
//
//Обработчик пришедших сообщений (port 80, POST, application/json, "http://127.0.0.1/")
//
app.post('/', function (req, res) {
	// console.log(req.body);
	if(req.body.message != "ping")
	{
		var msg = JSON.parse(req.body.message);
		console.log(msg);
	}
	else
	{
		console.log(req.body.message);
	}
	console.log("\n-----------------------------------------------------------------------------------------------------------------------------\n");
	if(counter >= 3)
	{
		// res.send('{"order":"quit"}');
		res.send('pong');
	}
		
	else
	{
		res.send('pong');
	}
		
	++counter;
});
var port = 80;
app.listen(port, function () {
  console.log('Server is listening port ' + port);
});
var timeStart = new Date();
//
//Завершение работы
//
process.on("SIGINT", function(){
	console.log("\n");
	db.close();
	var timeEnd = new Date();
	var log = "[Server time start]: " + timeStart.toLocaleString() + "\n[Server shutdown time]: " + timeEnd.toLocaleString() + "\n[Quantity of request]: " + counter;
	console.log(log);
	fs.writeFileSync("log.txt", log);
	process.exit();
});