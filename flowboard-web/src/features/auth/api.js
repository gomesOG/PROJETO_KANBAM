import { api } from '../../shared/api/client'

export const authApi = {
  register: (data) =>
    api.post('/auth/register', data).then((r) => r.data),
  login: (data) =>
    api.post('/auth/login', data).then((r) => r.data),
}
