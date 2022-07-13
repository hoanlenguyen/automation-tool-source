import axios from 'axios'
//import store from '@/store'
import { SnackbarProgrammatic as Snackbar } from 'buefy'
import { getToken } from '@/utils/auth'

// create an axios instance
const service = axios.create({
  baseURL: process.env.VUE_APP_BASE_API, // url = base url + request url
  // withCredentials: true, // send cookies when cross-domain requests
  timeout: 500000 // request timeout
})

// request interceptor
service.interceptors.request.use(
  config => {
    // do something before request is sent
    config.headers['Authorization'] = 'Bearer ' + getToken()
    // if (store.getters.token) {
    //   // let each request carry token
    //   // ['X-Token'] is a custom headers key
    //   // please modify it according to the actual situation
    //   config.headers['Authorization'] = 'Bearer ' + getToken()
    // }
    // config.params = {
    //   ...config.params,
    //   'culture': 'vi',
    //   'ui-culture': 'vi-VN'
    // }

    return config
  },
  error => {
    // do something with request error
    console.log(error) // for debug
    return Promise.reject(error)
  }
);

 

// response interceptor
service.interceptors.response.use(
  response => { return response },
  error => {    
    if (error.response) {      
      const data = error.response.data;
      if(data)
      {
        console.log(data.error);
        Snackbar.open({
          message: data.error,
          queue: false,
          duration: 4500,
          type: 'is-danger'
        });
      }
    }else{
      console.log('no error.response');
    }
    return Promise.reject(error)
  }
)

export default service
