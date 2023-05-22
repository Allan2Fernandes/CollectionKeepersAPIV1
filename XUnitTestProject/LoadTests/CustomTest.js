import http from 'k6/http'
import { sleep } from 'k6'
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js"

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<500'],
    }
};

export default () => {
    http.get('http://10.176.88.60:5001/api/User/GetAllUsers', { headers: { "Accept": "*/*" } });
    sleep(1);
};

export function handleSummary(data) {
    return {
        "K6Reports/Summary.html": htmlReport(data),
    };
}