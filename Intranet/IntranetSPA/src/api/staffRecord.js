import request from '@/utils/request'

export function getDetail(id) {
  return request({
    url: `/StaffRecord/${id}`,
    method: 'get'})
}

export function createOrUpdate(input) {
  return request({
    url: '/StaffRecord',
    method:input.id==0? 'post':'put',
    data:input    
  })
}

export function getList(input) {
  return request({
    url: '/StaffRecord/list',
    method: 'post',
    data:input    
  })
}

export function deleteData(id) {
  return request({
    url: `/StaffRecord/${id}`,
    method: 'delete'})
}

export function getEmployeeByBrand() {
  return request({
    url: `/StaffRecord/GetEmployeeByBrand`,
    method: 'get'})
}
 