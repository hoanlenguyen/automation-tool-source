import request from '@/utils/request'

export function importCustomerScore(inputParams, inputData) {
  return request({
    url: '/data/importCustomerScore',
    method: 'post',
    data: inputData,
    params:inputParams,
    headers: {'Content-Type': 'multipart/form-data'}
    })
}

export function compareCustomerMobiles(input) {
  return request({
    url: '/data/compareCustomerMobiles',
    method: 'post',
    data: input,
    headers: {'Content-Type': 'multipart/form-data'}
    })
}

export function importTotalPoints(input) {
  return request({
    url: '/data/importTotalPoints',
    method: 'post',
    data: input,
    headers: {'Content-Type': 'multipart/form-data'}
    })
}

export function getAdminScores() {
    return request({
      url: '/data/getAdminScores',
      method: 'get'
    })
  }

export function getAdminCampaigns() {
  return request({
    url: '/data/getAdminCampaigns',
    method: 'get'
  })
}

export function getSignalrTest(data) {
  return request({
    url: '/signalR/test',
    method: 'get',
    params: data
  })
}

export function importCleanedMobileNumberList(inputParams, inputData) {
  return request({
    url: '/data/importCleanedMobileNumberList',
    method: 'post',
    data: inputData,
    params:inputParams
    })
}

export function getTestError() {
  return request({
    url: '/auth/testError',
    method: 'get'
  })
}