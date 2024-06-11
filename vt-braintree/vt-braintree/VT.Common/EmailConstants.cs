﻿namespace VT.Common
{
    public static class EmailConstants
    {
        public static string AddUpdateCcInfo =
                @"<html>
                    <head>
                        <meta name='viewport' content='width=device-width' />
                        <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
                        <title>Add / Update my Credit Card Information</title>
                    </head>
                    <body>
                        <table>
                            <tr>
                                <td>
                                    Dear {NAME}, <br/>
                                    <br />
                                    Please click on the link below to add or update your credit card information with our payment processor. Rest assured your financial information is secure. The information you provide is not stored in our system anywhere, but rather is sent to BrainTree Payments, a third party payment processor. BrainTree Payments is a PayPal company and is an expert in keeping financial information secure and safe so you do not have to worry. <br/></br>
                                    <a href='{URL}'>Add / Update my Credit Card Information</a> 
                                    <br />
                                    <br />
                                    Thank you, <br/><br/>
                                    {ORGANISATION}
                                </td>
                            </tr>
                        </table>
                    </body>
               </html>";

        public static string GeneralEmailTemplate =
                @"Hi {CUSTOMER-NAME},
                <br/>
                <br/>
                Attached please find your {TITLE} for service completed on {DATE}. 
                <br/>
                <br/>
                Thank You,
                <br/>
                <br/>
                {ORGANISATION}";


        public static string VoidEmailTemplate = @"Hi {CUSTOMER-NAME},
                <br/>
                <br/>
                Attached please find {TITLE} that has been <b>voided</b>. Here is a summary:
                <br/>
                <br/>
                {INVOICE-DATE}
                {SERVICE-DATE}
                {VOID-DATE} 
                <br/>
                <br/>
                {MESSAGE}
                <br/>
                <br/>
                Thank you,
                <br/>
                <br/>
                {COMPANY-NAME}";

        public static string GeneralPdfTemplate =
            "<!doctype html><html><head><meta charset=\"utf-8\"><title>Invoice or Work Summary</title><style>/* reset */*{border: 0;box-sizing: content-box;color: inherit;font-family: inherit;font-size: inherit;font-style: inherit;font-weight: inherit;line-height: inherit;list-style: none;margin: 0;padding: 0;text-decoration: none;vertical-align: top;}/* content editable */*[contenteditable]{border-radius: 0.25em; min-width: 1em; outline: 0;}*[contenteditable]{cursor: pointer;}*[contenteditable]:hover, *[contenteditable]:focus, td:hover *[contenteditable], td:focus *[contenteditable], img.hover{background: #DEF; box-shadow: 0 0 1em 0.5em #DEF;}span[contenteditable]{display: inline-block;}/* heading */h1{font: bold 100% sans-serif; letter-spacing: 0.5em; text-align: center; text-transform: uppercase;}/* table */table{font-size: 75%; table-layout: fixed; width: 100%;}table{border-collapse: separate; border-spacing: 2px;}th, td{border-width: 1px; padding: 0.5em; position: relative; text-align: left;}th, td{border-radius: 0.25em; border-style: solid;}th{background: #EEE; border-color: #BBB;}td{border-color: #DDD;}/* page */html{font: 16px/1 'Open Sans', sans-serif; overflow: auto;}html{background: #fff; cursor: default;}body{box-sizing: border-box; height: 11in; margin: 0 auto; overflow: hidden; padding: 0.25in; width: 8.5in;}/*body{background: #FFF; border-radius: 1px; box-shadow: 0 0 1in -0.25in rgba(0, 0, 0, 0.5);}*/body{background: #FFF; border-radius: 0px;}/* header */header{margin: 0 0 3em;}header:after{clear: both; content: \"\"; display: table;}header h1{background: #1ab394; border-radius: 0.25em; color: #FFF; margin: 0 0 1em; padding: 0.5em 0;}header address{float: left; font-size: 75%; font-style: normal; line-height: 1.25; margin: 0 1em 1em 0;}header address p{margin: 0 0 0.25em;}header span, header img{display: block; float: right;}header span{margin: 0 0 1em 1em; max-height: 25%; max-width: 60%; position: relative;}header img{max-height: 100%; max-width: 100%;}header input{cursor: pointer; -ms-filter:\"progid:DXImageTransform.Microsoft.Alpha(Opacity=0)\"; height: 100%; left: 0; opacity: 0; position: absolute; top: 0; width: 100%;}/* article */article, article address, table.meta, table.inventory{margin: 0 0 3em;}article:after{clear: both; content: \"\"; display: table;}article h1{clip: rect(0 0 0 0); position: absolute;}article address{float: left; font-size: 125%; font-weight: bold;}/* table meta & balance */table.meta, table.balance{float: right; width: 36%;}table.meta:after, table.balance:after{clear: both; content: \"\"; display: table;}/* table meta */table.meta th{width: 40%;}table.meta td{width: 60%;}/* table items */table.inventory{clear: both; width: 100%;}table.inventory th{font-weight: bold; text-align: center;}table.inventory td:nth-child(1){width: 26%;}table.inventory td:nth-child(2){width: 38%;}table.inventory td:nth-child(3){text-align: right; width: 12%;}table.inventory td:nth-child(4){text-align: right; width: 12%;}table.inventory td:nth-child(5){text-align: right; width: 12%;}/* table balance */table.balance th, table.balance td{width: 50%;}table.balance td{text-align: right;}/* aside *//*aside h1{border: none; border-width: 0 0 1px; margin: 0 0 1em;}aside h1{border-color: #999; border-bottom-style: solid;}*/aside h1{background: #1ab394; border-radius: 0.25em; color: #FFF; margin: 0 0 1em; padding: 0.5em 0;}/* javascript */.add, .cut{border-width: 1px;display: block;font-size: .8rem;padding: 0.25em 0.5em;float: left;text-align: center;width: 0.6em;}.add, .cut{background: #9AF;box-shadow: 0 1px 2px rgba(0,0,0,0.2);background-image: -moz-linear-gradient(#00ADEE 5%, #0078A5 100%);background-image: -webkit-linear-gradient(#00ADEE 5%, #0078A5 100%);border-radius: 0.5em;border-color: #0076A3;color: #FFF;cursor: pointer;font-weight: bold;text-shadow: 0 -1px 2px rgba(0,0,0,0.333);}.add{margin: -2.5em 0 0;}.add:hover{background: #00ADEE;}.cut{opacity: 0; position: absolute; top: 0; left: -1.5em;}.cut{-webkit-transition: opacity 100ms ease-in;}tr:hover .cut{opacity: 1;}@media print{*{-webkit-print-color-adjust: exact;}html{background: none; padding: 0;}body{box-shadow: none; margin: 0;}span:empty{display: none;}.add, .cut{display: none;}}@page{margin: 0;}</style><!-- <link rel=\"license\" href=\"http://www.opensource.org/licenses/mit-license/\"><script src=\"script.js\"></script> --></head><body><header><h1>{TITLE}</h1> <address><p>{COMPANY-NAME}</p><p>{COMANPY-ADDRESS}</p><p>{COMPANY-TELEPHONE}</p><p>{COMPANY-EMAIL}</p></address><table class=\"meta\"></table> </header><article><h1>Recipient</h1><address> <p>{CUSTOMER-NAME}</p><p>{CUSTOMER-ADDRESS}</p><p>{CUSTOMER-TELEPHONE}</p><p>{CUSTOMER-EMAIL}</p></address><table class=\"meta\"><tr><th style=\"font-weight: bold\">Record Locator</th><td>{SERVICE-RECORD-ID}</td></tr><tr><th style=\"font-weight: bold\">Invoice Date</th><td>{INVOICE-DATE}</td></tr><tr><th style=\"font-weight: bold\">Service Date</th><td>{SERVICE-RECORD-DATE}</td></tr></table><table class=\"inventory\"><thead><tr><th width=\"50%\">Description</th><th width=\"35%\">Photo</th> <th width=\"15%\">Price</th></tr></thead><tbody>{INVOICE-ITEMS}</tbody></table> <table class=\"balance\"><tr><th style=\"font-weight: bold\">Total</th><td><span>{TOTAL}</span></td></tr><tr><th style=\"font-weight: bold\">Amount Paid</th><td><span>{AMOUNT-PAID}</span></td></tr><tr><th style=\"font-weight: bold\">Balance Due</th><td style=\"color: red; font-weight: bold\"><span>{BALANCE-DUE}</span></td></tr></table></article><aside><h1>Additional Notes</h1><div><p align=\"center\">{MESSAGE}<br><br></p><p align=\"center\" style=\"text-transform: uppercase; font-weight: bold\"><br><br>Thank you for your business!</p></div></aside></body></html>";

        public static string VoidPdfTemplate =
           "<!doctype html><html><head> <meta charset=\"utf-8\"> <title>Invoice or Work Summary</title> <style>/* reset */ *{border: 0; box-sizing: content-box; color: inherit; font-family: inherit; font-size: inherit; font-style: inherit; font-weight: inherit; line-height: inherit; list-style: none; margin: 0; padding: 0; text-decoration: none; vertical-align: top;}/* content editable */ *[contenteditable]{border-radius: 0.25em; min-width: 1em; outline: 0;}*[contenteditable]{cursor: pointer;}*[contenteditable]:hover, *[contenteditable]:focus, td:hover *[contenteditable], td:focus *[contenteditable], img.hover{background: #DEF; box-shadow: 0 0 1em 0.5em #DEF;}span[contenteditable]{display: inline-block;}/* heading */ h1{font: bold 100% sans-serif; letter-spacing: 0.5em; text-align: center; text-transform: uppercase;}/* table */ table{font-size: 75%; table-layout: fixed; width: 100%;}table{border-collapse: separate; border-spacing: 2px;}th, td{border-width: 1px; padding: 0.5em; position: relative; text-align: left;}th, td{border-radius: 0.25em; border-style: solid;}th{background: #EEE; border-color: #BBB;}td{border-color: #DDD;}/* page */ html{font: 16px/1 'Open Sans', sans-serif; overflow: auto;}html{background: #fff; cursor: default;}body{box-sizing: border-box; height: 11in; margin: 0 auto; overflow: hidden; padding: 0.25in; width: 8.5in;}/*body{background: #FFF; border-radius: 1px; box-shadow: 0 0 1in -0.25in rgba(0, 0, 0, 0.5);}*/ body{background: #FFF; border-radius: 0px;}/* header */ header:after{clear: both; content: \"\"; display: table;}header h1{background: #1ab394; border-radius: 0.25em; color: #FFF; margin: 0 0 1em; padding: 0.5em 0;}header address{float: left; font-size: 75%; font-style: normal; line-height: 1.25; margin: 0 1em 1em 0;}header address p{margin: 0 0 0.25em;}header span, header img{display: block; float: right;}header span{margin: 0 0 1em 1em; max-height: 25%; max-width: 60%; position: relative;}header img{max-height: 100%; max-width: 100%;}header input{cursor: pointer; -ms-filter: \"progid:DXImageTransform.Microsoft.Alpha(Opacity=0)\"; height: 100%; left: 0; opacity: 0; position: absolute; top: 0; width: 100%;}/* article */ article, article address, table.meta, table.inventory{margin: 0 0 3em;}article:after{clear: both; content: \"\"; display: table;}article h1{clip: rect(0 0 0 0); position: absolute;}article address{float: left; font-size: 125%; font-weight: bold;}/* table meta & balance */ table.meta, table.balance{float: right; width: 36%;}table.meta:after, table.balance:after{clear: both; content: \"\"; display: table;}/* table meta */ table.meta th{width: 40%;}table.meta td{width: 60%;}/* table items */ table.inventory{clear: both; width: 100%;}table.inventory th{font-weight: bold; text-align: center;}table.inventory td:nth-child(1){width: 26%;}table.inventory td:nth-child(2){width: 38%;}table.inventory td:nth-child(3){text-align: right; width: 12%;}table.inventory td:nth-child(4){text-align: right; width: 12%;}table.inventory td:nth-child(5){text-align: right; width: 12%;}/* table balance */ table.balance th, table.balance td{width: 50%;}table.balance td{text-align: right;}/* aside */ /*aside h1{border: none; border-width: 0 0 1px; margin: 0 0 1em;}aside h1{border-color: #999; border-bottom-style: solid;}*/ aside h1{background: #1ab394; border-radius: 0.25em; color: #FFF; margin: 0 0 1em; padding: 0.5em 0;}/* javascript */ .add, .cut{border-width: 1px; display: block; font-size: .8rem; padding: 0.25em 0.5em; float: left; text-align: center; width: 0.6em;}.add, .cut{background: #9AF; box-shadow: 0 1px 2px rgba(0, 0, 0, 0.2); background-image: -moz-linear-gradient(#00ADEE 5%, #0078A5 100%); background-image: -webkit-linear-gradient(#00ADEE 5%, #0078A5 100%); border-radius: 0.5em; border-color: #0076A3; color: #FFF; cursor: pointer; font-weight: bold; text-shadow: 0 -1px 2px rgba(0, 0, 0, 0.333);}.add{margin: -2.5em 0 0;}.add:hover{background: #00ADEE;}.cut{opacity: 0; position: absolute; top: 0; left: -1.5em;}.cut{-webkit-transition: opacity 100ms ease-in;}tr:hover .cut{opacity: 1;}@media print{*{-webkit-print-color-adjust: exact;}html{background: none; padding: 0;}body{box-shadow: none; margin: 0;}span:empty{display: none;}.add, .cut{display: none;}}@page{margin: 0;}#background{position: absolute; display: block; min-height: 50%; min-width: 50%; color: yellow;}#bg-text{color: lightgrey; font-size: 150px; transform: rotate(300deg); -webkit-transform: rotate(300deg);}#background > p{position: relative; right: -55px; letter-spacing:5px;}header h2{background: rgba(0, 0, 0, 0) none repeat scroll 0 0; border: medium none; font-size: 65px;color:red;font-weight:bold;}header td{border:none;}</style></head><body> <header> <h1>{TITLE}</h1> <address><p>{COMPANY-NAME}</p><p>{COMANPY-ADDRESS}</p><p>{COMPANY-TELEPHONE}</p><p>{COMPANY-EMAIL}</p></address><table class=\"meta\"><tbody> <tr> <td><h2 style=\"text-align:center;\">VOID</h2><p style=\"text-align:center; color:red; font-weight:bold;\">On {VOID-TIME}</p></td></tr></tbody></table> <table class=\"meta\"></table> </header> <article> <h1>Recipient</h1><address> <p>{CUSTOMER-NAME}</p><p>{CUSTOMER-ADDRESS}</p><p>{CUSTOMER-TELEPHONE}</p><p>{CUSTOMER-EMAIL}</p></address> <table class=\"meta\"><tr> <tr> <th style=\"font-weight: bold\">Record Locator</th> <td>{SERVICE-RECORD-ID}</td></tr><tr> <th style=\"font-weight: bold\">Invoice Date</th> <td>{INVOICE-DATE}</td></tr><tr> <th style=\"font-weight: bold\">Service Date</th> <td>{SERVICE-RECORD-DATE}</td></tr></table> <table class=\"inventory\"> <thead> <tr> <th width=\"50%\">Description</th> <th width=\"35%\">Photo</th> <th width=\"15%\">Price</th> </tr></thead> <tbody>{INVOICE-ITEMS}</tbody> </table> <table class=\"balance\"> <tr> <th style=\"font-weight: bold\">Total</th> <td><span>{TOTAL}</span></td></tr><tr> <th style=\"font-weight: bold\">Amount Paid</th> <td><span>{AMOUNT-PAID}</span></td></tr><tr> <th style=\"font-weight: bold\">Balance Due</th> <td style=\"color: red; font-weight: bold\"><span>{BALANCE-DUE}</span></td></tr></table> </article> <aside> <h1>Additional Notes</h1> <div> <p align=\"center\">{MESSAGE}<br><br></p><p align=\"center\" style=\"text-transform: uppercase; font-weight: bold\"> <br><br>Thank you for your business! </p></div></aside></body></html>";

    }
}
