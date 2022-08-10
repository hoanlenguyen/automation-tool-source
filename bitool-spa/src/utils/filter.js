import Vue from 'vue';
import moment from "moment";
Vue.filter('dateTime', function(value, outputFormat='DD-MM-YYYY hh:mm:ss') {
    if (!value) return '';
    return moment(value).format(outputFormat)
  });

Vue.filter('roundNumber', function(value, roundTo = 2) {
  if (isNaN(value)) return '';
  let roundNumber= 10**roundTo;
  return Math.round(value * roundNumber) / roundNumber;
});

Vue.filter('formattedNumber', function(value) {
  if (isNaN(value)) return '0';
  if (!value) return '0';
  return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
});