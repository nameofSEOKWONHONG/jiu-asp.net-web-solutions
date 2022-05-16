var result = '';
var currentDate = (new Date()).getTime();

var total = 0;
for (var i = 0; i < 1000000; i++)
{
    total += 1;
}
var dif = (new Date()).getTime() - currentDate;
var Seconds_from_T1_to_T2 = dif / 1000;
var Seconds_Between_Dates = Math.abs(Seconds_from_T1_to_T2);
result = `${Seconds_Between_Dates}`;