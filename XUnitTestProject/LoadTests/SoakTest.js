import http from 'k6/http'
import { sleep } from 'k6'
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js"

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    //vus: 10,
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<500'],
    },
    stages:
        [
            { duration: '50s', target: 400 }, // Simulate ramp up
            { duration: '20m', target: 400 }, // Maintain 
            { duration: '50s', target: 0 }, // Ramp down
        ]
};

export default () => {
    // batch fetch from multiple controllers
    http.batch([
        ['GET', 'http://10.176.88.60:5001/api/User/GetAllUsers', { headers: { "Accept": "*/*" } }],
        ['GET', 'http://10.176.88.60:5001/api/Main/CheckIfOnline', { headers: { "Accept": "*/*" } }]
        ])
    sleep(1);
};

export function handleSummary(data) {
    return {
        "K6Reports/SoakTestSummary.html": htmlReport(data),
    };
}