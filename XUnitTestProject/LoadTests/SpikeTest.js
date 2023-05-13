import http from 'k6/http'
import { sleep } from 'k6'
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js"

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    //vus: 10,
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<200'],
    },
    stages:
        [
            { duration: '10s', target: 15 }, // Below normal load
            { duration: '1m', target: 15 }, // Maintain this load for some time
            { duration: '10s', target: 300 }, // Ramp up to normal load
            { duration: '1m', target: 300 }, // Maintain normal load
            { duration: '10s', target: 15 }, // Beyond breaking point
            { duration: '20s', target: 15 }, // Maintain this load
            { duration: '5s', target: 0 }, // Ramp down gradually and check if the system recovers
        ]
};

export default () => {
    http.get('http://10.176.88.60:5001/api/User/GetAllUsers', { headers: { "Accept": "*/*" } });
    sleep(1);
};

export function handleSummary(data) {
    return {
        "K6Reports/SpikeTestSummary.html": htmlReport(data),
    };
}