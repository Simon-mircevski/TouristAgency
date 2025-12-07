import React, { useState, useEffect } from 'react';
import { destinationApi, tourApi, bookingApi, customerApi, guideApi } from '../services/api';
import { Destination, Tour, Booking, Customer, Guide } from '../types';

const Home: React.FC = () => {
  const [stats, setStats] = useState({
    destinations: 0,
    tours: 0,
    bookings: 0,
    customers: 0,
    guides: 0
  });
  const [recentDestinations, setRecentDestinations] = useState<Destination[]>([]);
  const [recentTours, setRecentTours] = useState<Tour[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        const [
          destinationsRes,
          toursRes,
          bookingsRes,
          customersRes,
          guidesRes
        ] = await Promise.all([
          destinationApi.getAll(),
          tourApi.getAll(),
          bookingApi.getAll(),
          customerApi.getAll(),
          guideApi.getAll()
        ]);

        setStats({
          destinations: destinationsRes.data.length,
          tours: toursRes.data.length,
          bookings: bookingsRes.data.length,
          customers: customersRes.data.length,
          guides: guidesRes.data.length
        });

        setRecentDestinations(destinationsRes.data.slice(0, 3));
        setRecentTours(toursRes.data.slice(0, 3));
      } catch (error) {
        console.error('Error fetching dashboard data:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchDashboardData();
  }, []);

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-gray-900 mb-4">
          Welcome to Tourist Agency
        </h1>
        <p className="text-xl text-gray-600">
          Discover amazing destinations and book your next adventure
        </p>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-6">
        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex items-center">
            <div className="p-3 rounded-full bg-primary-100 text-primary-600">
              <span className="text-2xl">🌍</span>
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">Destinations</p>
              <p className="text-2xl font-semibold text-gray-900">{stats.destinations}</p>
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex items-center">
            <div className="p-3 rounded-full bg-secondary-100 text-secondary-600">
              <span className="text-2xl">✈️</span>
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">Tours</p>
              <p className="text-2xl font-semibold text-gray-900">{stats.tours}</p>
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex items-center">
            <div className="p-3 rounded-full bg-yellow-100 text-yellow-600">
              <span className="text-2xl">📅</span>
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">Bookings</p>
              <p className="text-2xl font-semibold text-gray-900">{stats.bookings}</p>
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex items-center">
            <div className="p-3 rounded-full bg-green-100 text-green-600">
              <span className="text-2xl">👥</span>
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">Customers</p>
              <p className="text-2xl font-semibold text-gray-900">{stats.customers}</p>
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex items-center">
            <div className="p-3 rounded-full bg-purple-100 text-purple-600">
              <span className="text-2xl">👨‍🏫</span>
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">Guides</p>
              <p className="text-2xl font-semibold text-gray-900">{stats.guides}</p>
            </div>
          </div>
        </div>
      </div>

      {/* Recent Content */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Recent Destinations */}
        <div className="bg-white rounded-lg shadow">
          <div className="px-6 py-4 border-b border-gray-200">
            <h3 className="text-lg font-semibold text-gray-900">Recent Destinations</h3>
          </div>
          <div className="p-6">
            {recentDestinations.length > 0 ? (
              <div className="space-y-4">
                {recentDestinations.map((destination) => (
                  <div key={destination.id} className="flex items-center space-x-4">
                    <div className="flex-shrink-0">
                      <img
                        className="h-12 w-12 rounded-lg object-cover"
                        src={destination.imageUrl || 'https://via.placeholder.com/48'}
                        alt={destination.name}
                      />
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-gray-900 truncate">
                        {destination.name}
                      </p>
                      <p className="text-sm text-gray-500">
                        {destination.city}, {destination.country}
                      </p>
                    </div>
                    <div className="text-sm text-gray-500">
                      ${destination.price}
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-gray-500 text-center py-4">No destinations available</p>
            )}
          </div>
        </div>

        {/* Recent Tours */}
        <div className="bg-white rounded-lg shadow">
          <div className="px-6 py-4 border-b border-gray-200">
            <h3 className="text-lg font-semibold text-gray-900">Recent Tours</h3>
          </div>
          <div className="p-6">
            {recentTours.length > 0 ? (
              <div className="space-y-4">
                {recentTours.map((tour) => (
                  <div key={tour.id} className="flex items-center space-x-4">
                    <div className="flex-shrink-0">
                      <div className="h-12 w-12 rounded-lg bg-primary-100 flex items-center justify-center">
                        <span className="text-primary-600 text-lg">✈️</span>
                      </div>
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-gray-900 truncate">
                        {tour.name}
                      </p>
                      <p className="text-sm text-gray-500">
                        {new Date(tour.startDate).toLocaleDateString()} - {new Date(tour.endDate).toLocaleDateString()}
                      </p>
                    </div>
                    <div className="text-sm text-gray-500">
                      ${tour.price}
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-gray-500 text-center py-4">No tours available</p>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Home; 