export interface Destination {
  id: number;
  name: string;
  description: string;
  country: string;
  city: string;
  imageUrl: string;
  price: number;
  duration: number;
  createdAt: string;
  updatedAt: string;
}

export interface Customer {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address: string;
  dateOfBirth: string;
  createdAt: string;
  updatedAt: string;
}

export interface Guide {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  specialization: string;
  languages: string;
  experienceYears: number;
  isAvailable: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface Tour {
  id: number;
  name: string;
  description: string;
  destinationId: number;
  guideId: number;
  startDate: string;
  endDate: string;
  maxParticipants: number;
  currentParticipants: number;
  price: number;
  status: 'Scheduled' | 'InProgress' | 'Completed' | 'Cancelled';
  createdAt: string;
  updatedAt: string;
  destination?: Destination;
  guide?: Guide;
}

export interface Booking {
  id: number;
  customerId: number;
  tourId: number;
  bookingDate: string;
  travelDate: string;
  numberOfPeople: number;
  totalAmount: number;
  status: 'Pending' | 'Confirmed' | 'Cancelled' | 'Completed';
  specialRequests?: string;
  createdAt: string;
  updatedAt: string;
  customer?: Customer;
  tour?: Tour;
}

// Authentication types
export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  address?: string;
  dateOfBirth?: string;
  role: 'Customer' | 'Admin' | 'Guide';
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  phone?: string;
  address?: string;
  dateOfBirth?: string | null;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: UserInfo;
}

export interface UserInfo {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  phone?: string;
  address?: string;
  dateOfBirth?: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
} 