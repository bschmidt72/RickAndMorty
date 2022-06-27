interface CharRange {
  charCodeStart: number;
  charCodeEnd: number;
}

export class SanitizerBuilder {
  private static readonly CHAR_CODE_A: number = 65;
  private static readonly CHAR_CODE_Z: number = 90;
  private static readonly CHAR_CODE_a: number = 97;
  private static readonly CHAR_CODE_z: number = 122;
  private static readonly CHAR_CODE_0: number = 48;
  private static readonly CHAR_CODE_9: number = 57;
  private static readonly CHAR_CODE_SPACE: number = 32;

  private ranges: CharRange[] = [];

  constructor() {}

  public keepRange(
    charCodeStart: number,
    charCodeEnd: number
  ): SanitizerBuilder {
    this.ranges.push({ charCodeStart, charCodeEnd } as CharRange);
    return this;
  }

  public keepChar(charCode: number): SanitizerBuilder {
    this.ranges.push({ charCodeStart: charCode, charCodeEnd: charCode });
    return this;
  }

  public keepAlphaNumeric(): SanitizerBuilder {
    this.keepRange(SanitizerBuilder.CHAR_CODE_0, SanitizerBuilder.CHAR_CODE_9)
      .keepRange(SanitizerBuilder.CHAR_CODE_a, SanitizerBuilder.CHAR_CODE_z)
      .keepRange(SanitizerBuilder.CHAR_CODE_A, SanitizerBuilder.CHAR_CODE_Z);
    return this;
  }

  public keepSpaces(): SanitizerBuilder {
    this.keepChar(SanitizerBuilder.CHAR_CODE_SPACE);
    return this;
  }

  public build(): Sanitizer {
    return new Sanitizer(this.ranges);
  }
}

export class Sanitizer {
  public constructor(private ranges: CharRange[]) {}

  public sanitize(input: string): string {
    let sanitized = "";
    for (let charIndex = 0; charIndex < input.length; charIndex++) {
      const ascii = input.charCodeAt(charIndex);
      if (
        this.ranges.some(
          (range) => ascii >= range.charCodeStart && ascii <= range.charCodeEnd
        )
      ) {
        sanitized += input[charIndex];
      }
    }
    return sanitized;
  }

  public static builder(): SanitizerBuilder {
    return new SanitizerBuilder();
  }
}
