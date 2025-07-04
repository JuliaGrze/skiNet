import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';
import { PaymentSummarry } from '../models/order';

@Pipe({
  name: 'paymentCard'
})
export class PaymentCardPipe implements PipeTransform {

  transform(value?: ConfirmationToken['payment_method_preview'] | PaymentSummarry, ...args: unknown[]): unknown {
    //Jesli jest to ConfirmationToken['payment_method_preview']
    if(value && 'card' in value){
      const {brand, last4, exp_month, exp_year} = (value as ConfirmationToken['payment_method_preview']).card!
      return `${brand.toUpperCase()} **** **** **** ${last4}, Exp: ${exp_month}/${exp_year}`
    } //jesli jest to PaymentSummarry
    else if(value && 'expMonth' in value){
      const {brand, last4, expMonth, expYear} = value as PaymentSummarry
      return `${brand.toUpperCase()} **** **** **** ${last4}, Exp: ${expMonth}/${expYear}`
    }
    else
      return 'Unknow payment method'
  }

}
