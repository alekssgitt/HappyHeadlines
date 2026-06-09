import http from 'k6/http';
import { check } from 'k6';

export const options = {
  vus: 500,
  duration: '1m',
};

export default function () {
  const res = http.get('http://localhost:5006/api/articles/get-all-articles?continent=global');

  check(res, {
    'status is 200': (r) => r.status === 200,
  });
}
