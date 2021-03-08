'use strict';

const express = require('express');

// Constants
const PORT = 8080;
const HOST = '0.0.0.0';

// App
const app = express();

app.use(express.static('public'));


app.get('/', (req, res) => {
    res.sendFile('./index.html', { root: __dirname });
});

app.get('/env', (req,res)=>{
    res.send({
        baseUrl: process.env.TRAFIK_API_URL
    })
})

app.listen(PORT, HOST);
console.log(`Running on http://${HOST}:${PORT}`);