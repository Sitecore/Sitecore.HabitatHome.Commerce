import { Inject, LOCALE_ID, Pipe, PipeTransform } from '@angular/core';
import { formatNumber } from '../i18n/format-number';
import { NumberFormatStyle, findCurrencySymbol, getLocaleCurrencyName, getLocaleCurrencySymbol } from '../i18n/locale-data-api';
import { InvalidPipeArgumentError } from './invalid-pipe-argument-error';

/*
 * Formats a number as text. Group sizing and separator and other locale-specific
 * configurations are based on the active locale.
 * where `expression` is a number:
 *  - `digitInfo` is a `string` which has a following format:
 *     <code>{minIntegerDigits}.{minFractionDigits}-{maxFractionDigits}</code>
 *   - `minIntegerDigits` is the minimum number of integer digits to use. Defaults to `1`.
 *   - `minFractionDigits` is the minimum number of digits after fraction. Defaults to `0`.
 *   - `maxFractionDigits` is the maximum number of digits after fraction. Defaults to `3`.
 *  - `locale` is a `string` defining the locale to use (uses the current LOCALE_ID by
 * default)
 */
@Pipe({ name: 'scNumber' })
export class DecimalPipe implements PipeTransform {
  constructor( @Inject(LOCALE_ID) private _locale: string) { }

  transform(value: any, digits?: string, locale?: string): string | null {
    if (isEmpty(value)) { return null; }

    locale = locale || this._locale;

    const { str, error } = formatNumber(value, locale, NumberFormatStyle.Decimal, digits);

    if (error) {
      throw InvalidPipeArgumentError(DecimalPipe, error);
    }

    return str;
  }
}

/*
 * Use `currency` to format a number as currency.
 * - `currencyCode` is the [ISO 4217] currency code, such
 *    as `USD` for the US dollar and `EUR` for the euro.
 * - `display` indicates whether to show the currency symbol or the code.
 *   - `code`: use code (e.g. `USD`).
 *   - `symbol`(default): use symbol (e.g. `$`).
 *   - `symbol-narrow`: some countries have two symbols for their currency, one regular and one
 *   narrow (e.g. the canadian dollar CAD has the symbol `CA$` and the symbol-narrow `$`).
 *   If there is no narrow symbol for the chosen currency, the regular symbol will be used.
 * - `digitInfo` See DecimalPipe for detailed description.
 * - `locale` is a `string` defining the locale to use (uses the current LOCALE_ID by
 * default)
 */
@Pipe({ name: 'scCurrency' })
export class CurrencyPipe implements PipeTransform {
  constructor( @Inject(LOCALE_ID) private _locale: string) { }

  transform(
    value: any,
    display: 'code' | 'symbol' | 'symbol-narrow' | boolean = 'symbol',
    digits?: string,
    locale?: string): string | null {
    if (isEmpty(value)) { return null; }

    locale = locale || this._locale;

    if (typeof display === 'boolean') {
      if (<any>console && <any>console.warn) {
        console.warn(
          `Warning: The symbolDisplay option (third parameter) is a string.
            The accepted values are "code", "symbol" or "symbol-narrow".`);
      }
      display = display ? 'symbol' : 'code';
    }

    let isNegative = false;
    if (value.includes('(') && value.includes(')')) {
      value = value.replace('(', '').replace(')', '');
      isNegative = true;
    }

    let currency = value.substring(0, 3) || 'USD';
    if (display !== 'code') {
      currency = findCurrencySymbol(currency, display === 'symbol' ? 'wide' : 'narrow');
    }

    const { str, error } = formatNumber(value.substring(3, value.length), locale, NumberFormatStyle.Currency, digits, currency);

    if (error) {
      throw InvalidPipeArgumentError(CurrencyPipe, error);
    }

    return isNegative ? '(' + str + ')'  : str;
  }
}

function isEmpty(value: any): boolean {
  return value == null || value === '' || value !== value;
}
