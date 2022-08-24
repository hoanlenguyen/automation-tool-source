import request from '@/utils/request'

export function uploadFiles(inputParams, inputData) {
  return request({
    url: '/File/Upload',
    method: 'post',
    data: inputData,
    params:inputParams,
    headers: {'Content-Type': 'multipart/form-data'}
    })
}

 