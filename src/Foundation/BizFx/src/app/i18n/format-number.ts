import {NumberFormatStyle, NumberSymbol, getLocaleNumberFormat, getLocaleNumberSymbol} from './locale-data-api';

export const NUMBER_FORMAT_REGEXP = /^(\d+)?\.((\d+)(-(\d+))?)?$/;
const MAX_DIGITS = 22;
const DECIMAL_SEP = '.';
const ZERO_CHAR = '0';
const PATTERN_SEP = ';';
const GROUP_SEP = ',';
const DIGIT_CHAR = '#';
const CURRENCY_CHAR = '¤';
const PERCENT_CHAR = '%';

// tslint:disable-next-line:interface-over-type-literal
export type FormatNumberRes = {
  str: string | null,
  error?: string
};

/*
 * Transform a number to a locale string based on a style and a format
 */
export function formatNumber(
    value: number | string, locale: string, style: NumberFormatStyle, digitsInfo?: string | null,
    currency: string | null = null): FormatNumberRes {
  const res: FormatNumberRes = {str: null};
  const format = getLocaleNumberFormat(locale, style);
  let num;

  // Convert strings to numbers
  if (typeof value === 'string' && !isNaN(+value - parseFloat(value))) {
    num = +value;
  } else if (typeof value !== 'number') {
    res.error = `${value} is not a number`;
    return res;
  } else {
    num = value;
  }

  if (style === NumberFormatStyle.Percent) {
    num = num * 100;
  }

  const numStr = Math.abs(num) + '';
  const pattern = parseNumberFormat(format, getLocaleNumberSymbol(locale, NumberSymbol.MinusSign));
  let formattedText = '';
  let isZero = false;

  if (!isFinite(num)) {
    formattedText = getLocaleNumberSymbol(locale, NumberSymbol.Infinity);
  } else {
    const parsedNumber = parseNumber(numStr);

    let minInt = pattern.minInt;
    let minFraction = pattern.minFrac;
    let maxFraction = pattern.maxFrac;

    if (digitsInfo) {
      const parts = digitsInfo.match(NUMBER_FORMAT_REGEXP);
      if (parts === null) {
        res.error = `${digitsInfo} is not a valid digit info`;
        return res;
      }
      const minIntPart = parts[1];
      const minFractionPart = parts[3];
      const maxFractionPart = parts[5];
      if (minIntPart != null) {
        minInt = parseIntAutoRadix(minIntPart);
      }
      if (minFractionPart != null) {
        minFraction = parseIntAutoRadix(minFractionPart);
      }
      if (maxFractionPart != null) {
        maxFraction = parseIntAutoRadix(maxFractionPart);
      } else if (minFractionPart != null && minFraction > maxFraction) {
        maxFraction = minFraction;
      }
    }

    roundNumber(parsedNumber, minFraction, maxFraction);

    let digits = parsedNumber.digits;
    let integerLen = parsedNumber.integerLen;
    const exponent = parsedNumber.exponent;
    let decimals = [];
    isZero = digits.every(d => !d);

    // pad zeros for small numbers
    for (; integerLen < minInt; integerLen++) {
      digits.unshift(0);
    }

    // pad zeros for small numbers
    for (; integerLen < 0; integerLen++) {
      digits.unshift(0);
    }

    // extract decimals digits
    if (integerLen > 0) {
      decimals = digits.splice(integerLen, digits.length);
    } else {
      decimals = digits;
      digits = [0];
    }

    // format the integer digits with grouping separators
    const groups = [];
    if (digits.length >= pattern.lgSize) {
      groups.unshift(digits.splice(-pattern.lgSize, digits.length).join(''));
    }

    while (digits.length > pattern.gSize) {
      groups.unshift(digits.splice(-pattern.gSize, digits.length).join(''));
    }

    if (digits.length) {
      groups.unshift(digits.join(''));
    }

    const groupSymbol = currency ? NumberSymbol.CurrencyGroup : NumberSymbol.Group;
    formattedText = groups.join(getLocaleNumberSymbol(locale, groupSymbol));

    // append the decimal digits
    if (decimals.length) {
      const decimalSymbol = currency ? NumberSymbol.CurrencyDecimal : NumberSymbol.Decimal;
      formattedText += getLocaleNumberSymbol(locale, decimalSymbol) + decimals.join('');
    }

    if (exponent) {
      formattedText += getLocaleNumberSymbol(locale, NumberSymbol.Exponential) + '+' + exponent;
    }
  }

  if (num < 0 && !isZero) {
    formattedText = pattern.negPre + formattedText + pattern.negSuf;
  } else {
    formattedText = pattern.posPre + formattedText + pattern.posSuf;
  }

  if (style === NumberFormatStyle.Currency && currency !== null) {
    res.str = formattedText
                  .replace(CURRENCY_CHAR, currency)
                  // if we have 2 time the currency character, the second one is ignored
                  .replace(CURRENCY_CHAR, '');
    return res;
  }

  if (style === NumberFormatStyle.Percent) {
    res.str = formattedText.replace(
        new RegExp(PERCENT_CHAR, 'g'), getLocaleNumberSymbol(locale, NumberSymbol.PercentSign));
    return res;
  }

  res.str = formattedText;
  return res;
}

interface ParsedNumberFormat {
  minInt: number;
  // the minimum number of digits required in the fraction part of the number
  minFrac: number;
  // the maximum number of digits required in the fraction part of the number
  maxFrac: number;
  // the prefix for a positive number
  posPre: string;
  // the suffix for a positive number
  posSuf: string;
  // the prefix for a negative number (e.g. `-` or `(`))
  negPre: string;
  // the suffix for a negative number (e.g. `)`)
  negSuf: string;
  // number of digits in each group of separated digits
  gSize: number;
  // number of digits in the last group of digits before the decimal separator
  lgSize: number;
}

function parseNumberFormat(format: string, minusSign = '-'): ParsedNumberFormat {
  const p = {
    minInt: 1,
    minFrac: 0,
    maxFrac: 0,
    posPre: '',
    posSuf: '',
    negPre: '',
    negSuf: '',
    gSize: 0,
    lgSize: 0
  };

  const patternParts = format.split(PATTERN_SEP);
  const positive = patternParts[0];
  const negative = patternParts[1];

  const positiveParts = positive.indexOf(DECIMAL_SEP) !== -1 ?
      positive.split(DECIMAL_SEP) :
      [
        positive.substring(0, positive.lastIndexOf(ZERO_CHAR) + 1),
        positive.substring(positive.lastIndexOf(ZERO_CHAR) + 1)
      ],
        integer = positiveParts[0], fraction = positiveParts[1] || '';

  p.posPre = integer.substr(0, integer.indexOf(DIGIT_CHAR));

  for (let i = 0; i < fraction.length; i++) {
    const ch = fraction.charAt(i);
    if (ch === ZERO_CHAR) {
      p.minFrac = p.maxFrac = i + 1;
    } else if (ch === DIGIT_CHAR) {
      p.maxFrac = i + 1;
    } else {
      p.posSuf += ch;
    }
  }

  const groups = integer.split(GROUP_SEP);
  p.gSize = groups[1] ? groups[1].length : 0;
  p.lgSize = (groups[2] || groups[1]) ? (groups[2] || groups[1]).length : 0;

  if (negative) {
    const trunkLen = positive.length - p.posPre.length - p.posSuf.length,
          pos = negative.indexOf(DIGIT_CHAR);

    p.negPre = negative.substr(0, pos).replace(/'/g, '');
    p.negSuf = negative.substr(pos + trunkLen).replace(/'/g, '');
  } else {
    p.negPre = minusSign + p.posPre;
    p.negSuf = p.posSuf;
  }

  return p;
}

interface ParsedNumber {
  // an array of digits containing leading zeros as necessary
  digits: number[];
  // the exponent for numbers that would need more than `MAX_DIGITS` digits in `d`
  exponent: number;
  // the number of the digits in `d` that are to the left of the decimal point
  integerLen: number;
}

/*
 * Parse a number (as a string)
 * Significant bits of this parse algorithm came from https://github.com/MikeMcl/big.js/
 */
function parseNumber(numStr: string): ParsedNumber {
  let exponent = 0, digits, integerLen;
  let i, j, zeros;

  // Decimal point?
  if ((integerLen = numStr.indexOf(DECIMAL_SEP)) > -1) {
    numStr = numStr.replace(DECIMAL_SEP, '');
  }

  // Exponential form?
  if ((i = numStr.search(/e/i)) > 0) {
    // Work out the exponent.
    if (integerLen < 0) { integerLen = i; }
    integerLen += +numStr.slice(i + 1);
    numStr = numStr.substring(0, i);
  } else if (integerLen < 0) {
    // There was no decimal point or exponent so it is an integer.
    integerLen = numStr.length;
  }

  // Count the number of leading zeros.
  for (i = 0; numStr.charAt(i) === ZERO_CHAR; i++) { /* empty */
  }

  if (i === (zeros = numStr.length)) {
    // The digits are all zero.
    digits = [0];
    integerLen = 1;
  } else {
    // Count the number of trailing zeros
    zeros--;
    while (numStr.charAt(zeros) === ZERO_CHAR) { zeros--; }

    // Trailing zeros are insignificant so ignore them
    integerLen -= i;
    digits = [];
    // Convert string to array of digits without leading/trailing zeros.
    for (j = 0; i <= zeros; i++, j++) {
      digits[j] = +numStr.charAt(i);
    }
  }

  // If the number overflows the maximum allowed digits then use an exponent.
  if (integerLen > MAX_DIGITS) {
    digits = digits.splice(0, MAX_DIGITS - 1);
    exponent = integerLen - 1;
    integerLen = 1;
  }

  return {digits, exponent, integerLen};
}

/**
 * Round the parsed number to the specified number of decimal places
 * This function changes the parsedNumber in-place
 */
function roundNumber(parsedNumber: ParsedNumber, minFrac: number, maxFrac: number) {
  if (minFrac > maxFrac) {
    throw new Error(
        `The minimum number of digits after fraction (${minFrac}) is higher than the maximum (${maxFrac}).`);
  }

  const digits = parsedNumber.digits;
  let fractionLen = digits.length - parsedNumber.integerLen;
  const fractionSize = Math.min(Math.max(minFrac, fractionLen), maxFrac);

  // The index of the digit to where rounding is to occur
  let roundAt = fractionSize + parsedNumber.integerLen;
  const digit = digits[roundAt];

  if (roundAt > 0) {
    // Drop fractional digits beyond `roundAt`
    digits.splice(Math.max(parsedNumber.integerLen, roundAt));

    // Set non-fractional digits beyond `roundAt` to 0
    for (let j = roundAt; j < digits.length; j++) {
      digits[j] = 0;
    }
  } else {
    // We rounded to zero so reset the parsedNumber
    fractionLen = Math.max(0, fractionLen);
    parsedNumber.integerLen = 1;
    digits.length = Math.max(1, roundAt = fractionSize + 1);
    digits[0] = 0;
    for (let i = 1; i < roundAt; i++) { digits[i] = 0; }
  }

  if (digit >= 5) {
    if (roundAt - 1 < 0) {
      for (let k = 0; k > roundAt; k--) {
        digits.unshift(0);
        parsedNumber.integerLen++;
      }
      digits.unshift(1);
      parsedNumber.integerLen++;
    } else {
      digits[roundAt - 1]++;
    }
  }

  // Pad out with zeros to get the required fraction length
  for (; fractionLen < Math.max(0, fractionSize); fractionLen++) { digits.push(0); }

  // Do any carrying, e.g. a digit was rounded up to 10
  // tslint:disable-next-line:no-shadowed-variable
  const carry = digits.reduceRight(function(carry, d, i, digits) {
    d = d + carry;
    digits[i] = d % 10;
    return Math.floor(d / 10);
  }, 0);
  if (carry) {
    digits.unshift(carry);
    parsedNumber.integerLen++;
  }
}

export function parseIntAutoRadix(text: string): number {
  // tslint:disable-next-line:radix
  const result: number = parseInt(text);
  if (isNaN(result)) {
    throw new Error('Invalid integer literal when parsing ' + text);
  }
  return result;
}
