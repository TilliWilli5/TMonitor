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
var buttonPressedEventCount = 0;
app.post('/', function (req, res) {
	//console.log(req.body);
	var localButtonCount = 0;
	if(req.body.message != "ping")
	{
		var msg = JSON.parse(req.body.message);
		for(var iX=0; iX<msg.length; ++iX)
		{
			if(msg[iX].type == 1)
			{
				++localButtonCount;
				++buttonPressedEventCount;
			}
		}
		console.log("\n[ButtonEventCount]: +" + localButtonCount + "\n");
	}
	else
	{
		console.log("\n[Ping]\n");
	}
	/*
	if(req.body.message != "ping")
	{
		var msg = JSON.parse(req.body.message);
		for(var news in msg)
		{
			if(news.type == 1)
				++buttonPressedEventCount;
		}
	}
	*/
	++counter;
	res.send('ok');
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
	var log = "[Server time start]: " + timeStart.toLocaleString() + "\n[Server shutdown time]: " + timeEnd.toLocaleString() + "\n[Quantity of request]: " + counter + "\n[Quantity of ButtonEvent]: " + buttonPressedEventCount;
	console.log(log);
	fs.writeFileSync("log.txt", log);
	process.exit();
});