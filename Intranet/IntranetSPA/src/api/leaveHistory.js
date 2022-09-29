import request from '@/utils/request'

export function getDetail(id) {
  return request({
    url: `/LeaveHistory/${id}`,
    method: 'get'})
}

export function createOrUpdate(input) {
  return request({
    url: '/LeaveHistory',
    method:input.id==0? 'post':'put',
    data:input    
  })
}

export function getList(input) {
  return request({
    url: '/LeaveHistory/list',
    method: 'post',
    data:input    
  })
}

export function deleteData(id) {
  return request({
    url: `/LeaveHistory/${id}`,
    method: 'delete'})
}

export function getBrandDropdownByUser() {
  return request({
    url: 'LeaveHistory/GetBrandDropdownByUser',
    method: 'get'})
}

export function getBrandAndDepartmentDropdownByUser() {
  return request({
    url: 'LeaveHistory/GetBrandAndDepartmentDropdownByUser',
    method: 'get'})
}
