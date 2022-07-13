import request from '@/utils/request'
 
export function getCustomers(data) {
    return request({
      url: '/data/getCustomersBySP',
      method: 'post',
      data
    })
}

export function downloadCustomersBySP(data) {
  return request({
    url: '/data/downloadCustomersBySP',
    method: 'post',
    data
  })
}

export function getCustomerCount(data) {
  return request({
    url: '/data/getCustomerCountBySP',
    method: 'post',
    data
  })
}

export function assignCampaignToCustomers(data) {
    return request({
      url: '/data/assignCampaignToCustomers',
      method: 'post',
      data
    })
}