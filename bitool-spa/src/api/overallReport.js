import request from '@/utils/request'

export function getTotalLeads() {
  return request({
    url: '/overallReport/getTotalLeads',
    method: 'get'
  })
}

export function getTotalCountByRange(input) {
  return request({
    url: '/overallReport/getTotalCountByRange',
    method: 'get',             
    params: input                
  })
}


export function getTotalCountByLimitedRange(value) {
  console.log(value);
  return request({
    url: `/overallReport/getTotalCountByLimitedRange/${value}`,
    method: 'get'                
  })
}
