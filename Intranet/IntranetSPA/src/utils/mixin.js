import XLSX from "xlsx-js-style";
import moment from "moment";
import { getToken, setToken } from '@/utils/auth'

var myMixin = {
    methods: {
        exportExcelData(data, excelName, wch = null, isAddHeader=true) {
            const limitedSize= 1000000;
            var totalCount= data.length;
            var totalSheet=Math.ceil(totalCount/limitedSize);
            var wb = XLSX.utils.book_new(); // make Workbook of Excel
            for (let index = 0; index < totalSheet; index++) {
                let endRange=(index+1)*limitedSize;
                if(index==totalSheet-1)
                endRange = totalCount;
                
                var sheetData= data.slice(index*limitedSize, endRange);
                var workSheet =isAddHeader? XLSX.utils.json_to_sheet(sheetData): XLSX.utils.json_to_sheet(sheetData, {skipHeader: 1});
                if (wch && sheetData && sheetData.length) {
                    var size = Object.keys(data[0]).length;
                    if (size) {
                    var wscols = Array(size).fill({ wch: wch });
                    workSheet["!cols"] = wscols;
                    }
                }
                XLSX.utils.book_append_sheet(wb, workSheet, `Page-${index+1}`); // sheetAName is name of Worksheet                
            }           
          
            XLSX.writeFile(
                wb,
                `${excelName}_${moment(new Date()).format("DD/MM/YYYY hh:mm:ss")}.xlsx`
            ); // name of the file is 'book.xlsx'
        },
        openErrorMessage(code){
            if(code==401){
                setToken('');
                this.$buefy.snackbar.open({
                  message: 'Login failed!',
                  queue: false,
                  type: 'is-warning'
                });
             }else if(code==403){
                this.$buefy.snackbar.open({
                  message: 'No permission!',
                  queue: false,
                  duration: 5000,
                  type: 'is-danger'
                });
             }    
        },
        notifyErrorMessage(error){
            if(!error.response)
                return;
            //console.log(error.response);
            let code= error.response.status;
            if(code==401){
                setToken('');
                this.$buefy.snackbar.open({
                  message: 'Login failed!',
                  queue: false,
                  type: 'is-warning'
                });
             }else if(code==403){
                this.$buefy.snackbar.open({
                  message: 'No permission!',
                  queue: false,
                  duration: 5000,
                  type: 'is-danger'
                });
             }else if(code==500 &&error.response.data){
                let data =error.response.data
                this.$buefy.snackbar.open({
                    message: data.error,
                    queue: false,
                    duration: 5000,
                    type: 'is-danger'
                  });
             }    
        },
        convertDateToString(dateTime){
          if(!dateTime) return null;
          return `${dateTime.getFullYear()}-${('0' + (dateTime.getMonth()+1)).slice(-2)}-${('0' + dateTime.getDate()).slice(-2)}`
                +`T${('0' + (dateTime.getHours()+1)).slice(-2)}:${('0' + (dateTime.getMinutes()+1)).slice(-2)}:${('0' + (dateTime.getSeconds()+1)).slice(-2)}`;
        },
        checkValidPassword(value){
          let validMessages=[];
          let has_minimum_lenth = (value.length > 8);
          let has_number    = /\d/.test(value);
          let has_lowercase = /[a-z]/.test(value);
          let has_uppercase = /[A-Z]/.test(value);
          let has_special   = /[`!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/.test(value);
          if(!has_minimum_lenth)
            validMessages.push("Password must have at least 8 characters");

          if(!has_number)
            validMessages.push("Password must have at least 1 number");

          if(!has_uppercase)
            validMessages.push("Password must have at least 1 uppercase letter");

          if(!has_lowercase)
            validMessages.push("Password must have at least 1 lowercase letter");

          if(!has_special)
            validMessages.push("Password must have at least 1 special letter");
          
          return validMessages;
        },
        generateRandomPassword(pwdLen = 10){
          // var pwdChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz~!@-#$";
          // var randPassword = Array(pwdLen).fill(pwdChars).map(function(x) { return x[Math.floor(Math.random() * x.length)] }).join('');
          // return randPassword;

          const lowerCase = 'abcdefghijklmnopqrstuvwxyz'
          const upperCase = lowerCase.toUpperCase()
          const numberChars = '0123456789'
          const specialChars = '!#@$%+?^&*()'

          let generatedPassword = ''
          let restPassword = ''

          const restLength = pwdLen % 4 
          const usableLength = pwdLen - restLength
          const generateLength = usableLength / 4

          const randomString = (char) => {
            return char[Math.floor(Math.random() * (char.length))]
          }
          for (let i = 0; i <= generateLength - 1; i++) {
            generatedPassword += `${randomString(lowerCase)}${randomString(upperCase)}${randomString(numberChars)}${randomString(specialChars)}`
          }

          for (let i = 0; i <= restLength - 1; i++) {
            restPassword += randomString([...lowerCase, ...upperCase, ...numberChars, ...specialChars])
          }

          return generatedPassword + restPassword
        }
    }
}
export default myMixin;