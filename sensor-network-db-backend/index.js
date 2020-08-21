const express = require('express');
const bodyParser = require('body-parser');
const mongoose = require('mongoose');
const jwt = require('jsonwebtoken');
const cors = require('cors');

const Facility = require('./model/facility');
const LiveFeed = require('./model/live_feed');
const MaintRecord = require('./model/maint_record');

mongoose.connect('mongodb://localhost:27017/data', {
    useNewUrlParser: true,
    useUnifiedTopology: true,
}, () => {
    console.log(`Database connected.\n`);
});

const app = express();
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

const secret = 'test'; //please don't be pissed off

const router = express.Router();

router.use('/', (req, res, next) => {
    console.log(`${new Date()} : ${req.method}\t${req.url}\n`);
    next();
});

router.route('/facility/:_id')
    .put((req, res) => {

        // console.log(
        // `name: ${req.body.name}
        // _id: ${req.body._id}
        // type: ${req.body.type}
        // dop: ${req.body.dop}`);

        const facility = new Facility({
            name: req.body.name,
            _id: req.params._id,
            type: req.body.type,
            dop: new Date(req.body.dop),
        });

        facility.save().then(result => {
            if (facility.isNew == false)
                return res.json({ _id: result._id });
        })
            .catch(err => {
                return res.status(500).send(err.message);
            });
    })

    .get((req, res) => {
        Facility.findById(req.params._id).then(facility => {
            if (!facility) return res.status(404).json('Facility Not Found');

            return res.json(facility);
        })
    })

    .post((req, res) => {
        Facility.findById(req.params._id).then(facility => {
            if (!facility) return res.status(404).json('Facility Not Found');

            req.body.status == 0 ? facility.status = 'Stopped' : facility.status = 'Running';

            facility.save().then(result => {
                return res.json(`facility (${result._id})'s status has been updated to ${result.status}`);
            })
                .catch(err => {
                    return res.status(500).json(err.message);
                });
        });
    });

router.route('/facility/maintenance/:_id')
    .put((req, res) => {
        Facility.findById(req.params._id).then(facility => {
            if (!facility) return res.status(404).json('Desired Facility Not Found');

            const maintRecord = new MaintRecord({
                facility_id: req.params._id,
                timestamp: new Date(),
                technician: req.body.technician,
                remarks: req.body.remarks,
            });

            maintRecord.save().then(result => {
                return res.json('maintenance recorded');
            })
                .catch(err => {
                    return res.status(500).json(err.message);
                })
        })
        
    })

    .get((req, res) => {
        MaintRecord.find({facility_id: req.params._id}).sort('-date').limit(5).then(list => {
            if (!list) return res.status(404).json('Desired Facility Does Not Have Any Recorded Maintenance Yet');

            return res.json(list);
        })
    })

router.route('/facility/live_feed/:_id')
    .put((req, res) => {
        Facility.findById(req.params._id).then(facility => {
            if (!facility) return res.status(404).json('Desired Facility Not Found');

            const liveFeed= new LiveFeed({
                facility_id: req.params._id,
                timestamp: new Date(),
                wattage: req.body.wattage,
                temp: req.body.temp,
            });

            liveFeed.save().then(result => {
                return res.json('livefeed recorded');
            })
                .catch(err => {
                    return res.status(500).json(err.message);
                })
        })
    })

    .get((req, res) => {
        LiveFeed.find({facility_id: req.params._id}).sort({timestamp: -1}).limit(1).then(livefeed => {
            if (!livefeed) return res.status(404).json('Something Went Wrong');

            return res.json(livefeed[0]);
        })
    })


app.use('/api', router);
app.use(cors());
app.listen(8080);
console.log('Listening on port: 8080');


