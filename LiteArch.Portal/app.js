'use strict';

const express = require('express');

// Constants
const PORT = 8080;
const HOST = '0.0.0.0';

// App
const app = express();

app.use(express.static('public'));

var fs = require('fs')
fs.readFile('index.html', 'utf8', function (err,data) {
    if (err) {
        return console.log(err);
    }
    var result = data.replace(/<%=TRAFIK_PORTAL_PREFIX%>/g, process.env.TRAFIK_PORTAL_PREFIX);

    fs.writeFile('index.html', result, 'utf8', function (err) {
        if (err) return console.log(err);
    });
});

app.get('/', (req, res) => {
    res.sendFile('./index.html', { root: __dirname });
});

app.get('/env', (req,res)=>{
    res.send({
        baseUrl: process.env.TRAFIK_API_URL,
        portalPrefix: process.env.TRAFIK_PORTAL_PREFIX
    })
})

app.listen(PORT, HOST);
console.log(`Running on http://${HOST}:${PORT}`);