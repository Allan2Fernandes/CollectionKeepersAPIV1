import http from 'k6/http'
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js"

export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<200'],
    }
};

export default () => {
    var res = http.get('http://10.176.129.17:5001/api/User/GetAllUsers', { headers: { "Accept": "*/*" } });
};

export function handleSummary(data) {
    return {
        "summary.html": htmlReport(data),
    };
}