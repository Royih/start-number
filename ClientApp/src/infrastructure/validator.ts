const passwordPattern = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9])(?!.*\s).{6,}$/;
const passwordPatternFailureMessage = "The password must be at least 6 characters and contain a lower, upper, digit and a special character.";

const emailPattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
const emailPatternFailureMessage = "Please enter a valid email address";

export interface IValidationElement {
    name: string;
    dirty: boolean;
    wasVisited: boolean;
    value?: any;
    validationOptions: IValidationOptions;
    errors: string[];
    validate(allElements: IValidationElement[]): void;
}
export interface IValidationOptions {
    required?: boolean;
    min?: number;
    max?: number;
    minLength?: number;
    maxLength?: number;
    pattern?: string;
    email?: boolean;
    password?: boolean;
    valueMustMachOtherElementWithName?: string;
}
export class ValidationElement implements IValidationElement {
    name = "";
    dirty = false;
    wasVisited = false;
    value?: any;
    validationOptions: IValidationOptions;
    errors: string[];

    constructor(name: string, value: any, validationOptions: IValidationOptions) {
        this.name = name;
        this.value = value;
        this.validationOptions = validationOptions;
        this.errors = [];
    }
    validate = (allElements: IValidationElement[]): void => {
        this.errors = [];
        if (this.validationOptions.required && !this.value) {
            this.errors.push("This is a required field");
        }
        if (this.validationOptions.minLength && !this.validationOptions.maxLength && this.value && this.value.length < this.validationOptions.minLength) {
            this.errors.push(`Enter a minimum of ${this.validationOptions.minLength} characters`);
        } else if (this.validationOptions.maxLength && !this.validationOptions.minLength && this.value && this.value.length > this.validationOptions.maxLength) {
            this.errors.push(`Enter a maximum of ${this.validationOptions.maxLength} characters`);
        } else if (
            this.validationOptions.maxLength &&
            this.validationOptions.minLength &&
            this.value &&
            (this.value.length < this.validationOptions.minLength || this.value.length > this.validationOptions.maxLength)
        ) {
            this.errors.push(`Enter between ${this.validationOptions.minLength} and ${this.validationOptions.maxLength} characters`);
        }
        if (this.validationOptions.email && this.value && !emailPattern.test(this.value)) {
            this.errors.push(emailPatternFailureMessage);
        } else if (this.validationOptions.password && this.value && !passwordPattern.test(this.value)) {
            this.errors.push(passwordPatternFailureMessage);
        } else if (this.validationOptions.pattern && this.value) {
            const re = new RegExp(this.validationOptions.pattern);
            if (!re.test(this.value)) {
                this.errors.push("Invalid value");
            }
        }
        if (this.validationOptions.valueMustMachOtherElementWithName) {
            const otherElement = allElements.find(x => x.name === this.validationOptions.valueMustMachOtherElementWithName);
            if (this.value !== otherElement?.value) {
                this.errors.push("The values does not match");
            }
        }
    };
}

export class FormValidator {
    valid = true;
    dirty = false;
    allowSave = false;
    elements: IValidationElement[];

    constructor(elements: IValidationElement[]) {
        this.elements = elements;
        for (const elem of this.elements) {
            elem.validate(this.elements);
        }
        this.validate();
    }
    updateValue = (name: string, value: any): FormValidator => {
        const element = this.elements.find(x => x.name === name);
        if (element && element.value !== value) {
            element.value = value;
            element.dirty = true;
            element.validate(this.elements);
        }
        return this.validate();
    };
    setPristine = (): FormValidator => {
        for (const elem of this.elements) {
            elem.dirty = false;
            elem.wasVisited=false;
        }
        return this.validate();
    };
    setDirty = (): FormValidator => {
        this.dirty = true;
        this.allowSave = this.valid;
        return { ...this };
    };
    onBlur = (name: string): FormValidator => {
        const element = this.elements.find(x => x.name === name);
        if (element) {
            element.wasVisited = true;
            element.validate(this.elements);
        }
        return { ...this };
    };
    reportError = (name: string): boolean => {
        const element = this.elements.find(x => x.name === name);
        if (element) {
            return element.errors && element.errors.length > 0 && element.wasVisited;
        }
        return false;
    };
    errorMessage = (name: string): string => {
        const element = this.elements.find(x => x.name === name);
        if (element) {
            if (element.errors && element.errors.length === 0) {
                return "";
            }
            if (element.wasVisited && element.errors) {
                return element.errors.join(", ");
            }
            return "";
        }
        return "Invalid input";
    };

    private validate(): FormValidator {
        const elementesWithErrors = this.elements.filter(x => x.errors && x.errors.length > 0);
        const dirtyElements = this.elements.filter(x => x.dirty);
        this.valid = !elementesWithErrors || elementesWithErrors.length === 0;
        this.dirty = dirtyElements && dirtyElements.length > 0;
        this.allowSave = this.dirty && this.valid;
        return { ...this };
    }
}
