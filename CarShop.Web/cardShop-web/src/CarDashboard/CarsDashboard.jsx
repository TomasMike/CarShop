import { useState, useEffect } from 'react';
import { responseError } from '../App/App';

export function CarsDashboard()
{
    const [cars, setCars] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [name, setName] = useState('');
    const [price, setPrice] = useState('');
    const [saving, setSaving] = useState(false);

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

    return (
        <div className="dashboard-table-wrap">
             <h1>Cars Dashboard</h1>
            <table>
                <thead>
                    <tr>
                        <th>Car</th>
                        <th>Price</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {cars.map((car) => (
                        <tr key={car.id}>
                            <td>{car.model}-{car.carBrand}</td>
                            <td>{Number(car.price).toLocaleString(undefined, { style: 'currency', currency: 'EUR' })}</td>
                            <td></td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>

        // <div className="garage-content">
        //     <div aria-labelledby="car-list-title">
        //         <h2 id="car-list-title" className="section-title">Inventory</h2>
        //         {loading ? <p className="subtext">Loading cars…</p> : cars.length === 0 ? (
        //             <p className="subtext">No cars have been added yet.</p>
        //         ) : (
        //             <ul className="car-list">
        //                 {cars.map((car) => (
        //                     <li className="car-row" key={car.id}>
        //                         <span>{car.name}</span>
        //                         <strong>{Number(car.price).toLocaleString(undefined, { style: 'currency', currency: 'USD' })}</strong>
        //                     </li>
        //                 ))}
        //             </ul>
        //         )}
        //     </div>

        //     <div className="add-car-section" aria-labelledby="add-car-title">
        //         <h2 id="add-car-title" className="section-title">Add a car</h2>
        //         <form className="car-form" onSubmit={addCar}>
        //             <label htmlFor="car-name">Name</label>
        //             <input
        //                 id="car-name"
        //                 value={name}
        //                 onChange={(event) => setName(event.target.value)}
        //                 required />
        //             <label htmlFor="car-price">Price</label>
        //             <input
        //                 id="car-price"
        //                 type="number"
        //                 min="0"
        //                 step="0.01"
        //                 value={price}
        //                 onChange={(event) => setPrice(event.target.value)}
        //                 required />
        //             {error && <p className="error" role="alert">{error}</p>}
        //             <button className="primary-button" type="submit" disabled={saving}>
        //                 {saving ? 'Adding…' : 'Add car'}
        //             </button>
        //         </form>
        //     </div>
        // </div>
    );
}
