import './App.css';
import { CarsDashboard } from '../CarDashboard/CarsDashboard';
import { LoginPanel, responseError } from '../LoginPanel/LoginPanel';

function App()
{
    return (
        <div className="app-shell">
            <LoginPanel />


            <div className="dashboard-section">
               
                <CarsDashboard />
            </div>
        </div>
    );
}

export { responseError };
export default App;
