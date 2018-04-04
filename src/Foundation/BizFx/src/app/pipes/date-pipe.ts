import { Inject, LOCALE_ID, Pipe, PipeTransform } from '@angular/core';
import { formatDate } from '@sitecore/bizfx';
import { InvalidPipeArgumentError } from './invalid-pipe-argument-error';

export const ISO8601_DATE_REGEX =
  /^(\d{4})-?(\d\d)-?(\d\d)(?:T(\d\d)(?::?(\d\d)(?::?(\d\d)(?:\.(\d+))?)?)?(Z|([+-])(\d\d):?(\d\d))?)?$/;
//    1        2       3         4          5          6          7          8  9     10      11

// clang-format off
/*
 * Where:
 * - `expression` is a date object or a number (milliseconds since UTC epoch) or an ISO string
 * (https://www.w3.org/TR/NOTE-datetime).
 * - `format` indicates which date/time components to include. The format can be predefined as
 *   shown below (all examples are given for `en-US`) or custom as shown in the table.
 *   - `'short'`: equivalent to `'M/d/yy, h:mm a'` (e.g. `6/15/15, 9:03 AM`)
 *   - `'medium'`: equivalent to `'MMM d, y, h:mm:ss a'` (e.g. `Jun 15, 2015, 9:03:01 AM`)
 *   - `'long'`: equivalent to `'MMMM d, y, h:mm:ss a z'` (e.g. `June 15, 2015 at 9:03:01 AM GMT+1`)
 *   - `'full'`: equivalent to `'EEEE, MMMM d, y, h:mm:ss a zzzz'` (e.g. `Monday, June 15, 2015 at
 * 9:03:01 AM GMT+01:00`)
 *   - `'shortDate'`: equivalent to `'M/d/yy'` (e.g. `6/15/15`)
 *   - `'mediumDate'`: equivalent to `'MMM d, y'` (e.g. `Jun 15, 2015`)
 *   - `'longDate'`: equivalent to `'MMMM d, y'` (e.g. `June 15, 2015`)
 *   - `'fullDate'`: equivalent to `'EEEE, MMMM d, y'` (e.g. `Monday, June 15, 2015`)
 *   - `'shortTime'`: equivalent to `'h:mm a'` (e.g. `9:03 AM`)
 *   - `'mediumTime'`: equivalent to `'h:mm:ss a'` (e.g. `9:03:01 AM`)
 *   - `'longTime'`: equivalent to `'h:mm:ss a z'` (e.g. `9:03:01 AM GMT+1`)
 *   - `'fullTime'`: equivalent to `'h:mm:ss a zzzz'` (e.g. `9:03:01 AM GMT+01:00`)
 *  - `timezone` to be used for formatting. It understands UTC/GMT and the continental US time zone
 *  abbreviations, but for general use, use a time zone offset, for example,
 *  `'+0430'` (4 hours, 30 minutes east of the Greenwich meridian)
 *  If not specified, the local system timezone of the end-user's browser will be used.
 *  - `locale` is a `string` defining the locale to use (uses the current LOCALE_ID by
 * default)
 *
 *  When the expression is a ISO string without time (e.g. 2016-09-19) the time zone offset is not
 * applied and the formatted text will have the same day, month and year of the expression.
 */
@Pipe({ name: 'scDate', pure: true })
export class DatePipe implements PipeTransform {
  constructor( @Inject(LOCALE_ID) private locale: string) { }

  transform(value: any, format = 'mediumDate', timezone?: string, locale?: string): string | null {
    if (value == null || value === '' || value !== value) { return null; }

    if (typeof value === 'string') {
      value = value.trim();
      value.replace(/:([^:]*)$/, '$1');
    }

    let date: Date;
    if (isDate(value)) {
      date = value;
    } else if (!isNaN(value - parseFloat(value))) {
      date = new Date(parseFloat(value));
    } else if (typeof value === 'string' && /^(\d{4}-\d{1,2}-\d{1,2})$/.test(value)) {
      /**
      * For ISO Strings without time the day, month and year must be extracted from the ISO String
      * before Date creation to avoid time offset and errors in the new Date.
      * If we only replace '-' with ',' in the ISO String ("2015,01,01"), and try to create a new
      * date, some browsers (e.g. IE 9) will throw an invalid Date error
      * If we leave the '-' ("2015-01-01") and try to create a new Date("2015-01-01") the timeoffset
      * is applied
      * Note: ISO months are 0 for January, 1 for February, ...
      */
      const [y, m, d] = value.split('-').map((val: string) => +val);
      date = new Date(y, m - 1, d);
    } else {
      date = new Date(value);
    }

    if (!isDate(date)) {
      let match: RegExpMatchArray | null;
      if ((typeof value === 'string') && (match = value.match(ISO8601_DATE_REGEX))) {
        date = isoStringToDate(match);
      } else {
        throw InvalidPipeArgumentError(DatePipe, value);
      }
    }

    return formatDate(date, format, locale || this.locale, timezone);
  }
}

export function isoStringToDate(match: RegExpMatchArray): Date {
  const date = new Date(0);
  let tzHour = 0;
  let tzMin = 0;
  const dateSetter = match[8] ? date.setUTCFullYear : date.setFullYear;
  const timeSetter = match[8] ? date.setUTCHours : date.setHours;

  if (match[9]) {
    tzHour = +(match[9] + match[10]);
    tzMin = +(match[9] + match[11]);
  }
  dateSetter.call(date, +(match[1]), +(match[2]) - 1, +(match[3]));
  const h = +(match[4] || '0') - tzHour;
  const m = +(match[5] || '0') - tzMin;
  const s = +(match[6] || '0');
  const ms = Math.round(parseFloat('0.' + (match[7] || 0)) * 1000);
  timeSetter.call(date, h, m, s, ms);
  return date;
}

function isDate(value: any): value is Date {
  return value instanceof Date && !isNaN(value.valueOf());
}
