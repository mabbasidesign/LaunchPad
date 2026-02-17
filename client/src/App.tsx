import { Login } from './pages/Login'
import { Books } from './pages/Books'
import './App.css'

function App() {
  const path = window.location.pathname;

  // Simple client-side routing
  if (path === '/books') {
    return <Books />;
  }

  return <Login />;
}

export default App
