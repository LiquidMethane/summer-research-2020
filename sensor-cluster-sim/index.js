const fetch = require('node-fetch');

const url = 'http://localhost:8080/api/facility/';

(function () {
    setInterval(() => {

        updateLivefeed(123456789, 250, 45);
        updateLivefeed(987654321, 1000, 65);
        updateLivefeed(789456123, 500, 50);

    }, 1000);
})();

var updateLivefeed = (facility_id, wattage, temp) => {

    const body = {
        wattage: (wattage + Math.random() * 2 - 1).toFixed(1),
        temp: (temp + Math.random() * 6 - 3).toFixed(1),
    };

    fetch(url + `live_feed/${facility_id}/`,
            {
                method: 'PUT',
                mode: 'cors',
                headers: {
                    "Content-type": "application/json"
                },
                body: JSON.stringify(body),
            })
            .then(res => {
                if (res.status != 200)
                    return res.json().then(data => {
                        console.log(data);
                    })

                res.json().then(data => {
                    console.log(data);
                })
            })
            .catch(err => {
                console.log(err.message);
            })
}
