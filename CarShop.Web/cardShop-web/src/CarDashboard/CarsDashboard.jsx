import { useState, useEffect } from 'react';
import { responseError } from '../App/App';

export function CarsDashboard({ isAuthenticated })
{
    const [cars, setCars] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [name, setName] = useState('');
    const [price, setPrice] = useState('');
    const [saving, setSaving] = useState(false);
    const [orderingCarId, setOrderingCarId] = useState(null);
    const [orderMessage, setOrderMessage] = useState('');

    useEffect(() =>
    {
        let cancelled = false;

        async function loadCars()
        {
            try
            {
                const response = await fetch('/api/car');
                if (!response.ok) throw new Error(await responseError(response));
                const result = await response.json();
                if (!cancelled) setCars(result);
            } catch (loadError)
            {
                if (!cancelled) setError(loadError.message || 'Could not load cars.');
            } finally
            {
                if (!cancelled) setLoading(false);
            }
        }

        loadCars();
        return () => { cancelled = true; };
    }, []);

    async function addCar(event)
    {
        event.preventDefault();
        setSaving(true);
        setError('');

        try
        {
            const response = await fetch('/api/car', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${sessionStorage.getItem('carshop.accessToken')}`,
                },
                body: JSON.stringify({ name, price: Number(price) }),
            });
            if (!response.ok) throw new Error(await responseError(response));

            const car = await response.json();
            setCars((currentCars) => [...currentCars, car]);
            setName('');
            setPrice('');
        } catch (saveError)
        {
            setError(saveError.message || 'Could not add the car.');
        } finally
        {
            setSaving(false);
        }
    }

    async function createOrder(car)
    {
        setOrderingCarId(car.id);
        setOrderMessage('');
        setError('');

        try
        {
            const response = await fetch('/api/orders', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${sessionStorage.getItem('carshop.accessToken')}`,
                },
                body: JSON.stringify({ carId: car.id }),
            });
            if (!response.ok) throw new Error(await responseError(response));

            const order = await response.json();
            setOrderMessage(`Order #${order.id} created successfully.`);
        } catch (orderError)
        {
            setError(orderError.message || 'Could not create the order.');
        } finally
        {
            setOrderingCarId(null);
        }
    }

    return (
        <div className="dashboard-table-wrap">
            <h1>Cars Dashboard</h1>
            {loading && <p>Loading cars…</p>}
            {error && <p className="error" role="alert">{error}</p>}
            {orderMessage && <p role="status">{orderMessage}</p>}
            <table>
                <thead>
                    <tr>
                        <th>Car</th>
                        <th>Price</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    {cars.map((car) => (
                        <tr key={car.id}>
                            <td>{car.model} - {car.carBrand?.name}</td>
                            <td>{Number(car.price).toLocaleString(undefined, { style: 'currency', currency: 'EUR' })}</td>
                            <td>
                               {/*  {isAuthenticated ? (
                                    <button
                                        type="button"
                                        onClick={() => createOrder(car)}
                                        disabled={orderingCarId !== null}
                                    >
                                        {orderingCarId === car.id ? 'Ordering…' : 'Order'}
                                    </button>
                                ) : 'Sign in to order'} */}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            {!loading && cars.length === 0 && <p>No cars are currently listed.</p>}
        </div>
    );
}
