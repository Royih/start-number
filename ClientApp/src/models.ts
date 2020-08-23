export interface ActiveEventDto {
  tenantKey: string;
  name: string;
  eventId: string;
}

export interface SignUpDto {
  tenantKey: string;
  firstName: string;
  surName: string;
  email: string;
  allowUsToContactPersonByEmail: boolean;
  previouslyParticipated: boolean;
}

export interface CommandResultDto<T> {
  data: T;
  success: boolean;
  errorMessages: string[];
  messages: string[];
}
export interface SignUpsForEventDto {
  startNumber: number;
  personId: string;
  firstName: string;
  surName: string;
  email: string;
  allowUsToContactPersonByEmail: boolean;
}
