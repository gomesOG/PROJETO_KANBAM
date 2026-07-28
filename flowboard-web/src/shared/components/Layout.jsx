import { Link, useNavigate } from 'react-router-dom'
import { Outlet } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'
import { LogOut, LayoutDashboard } from 'lucide-react'

export function Layout() {
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)
  const logout = useAuthStore((s) => s.logout)

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const initials = user?.name
    ? user.name.split(' ').map((n) => n[0]).slice(0, 2).join('').toUpperCase()
    : '?'

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col">
      <header className="bg-white border-b border-gray-200 h-14 flex items-center px-6 gap-4">
        <Link
          to="/projects"
          className="flex items-center gap-2 text-blue-600 font-semibold text-lg hover:text-blue-700 transition-colors"
        >
          <LayoutDashboard size={20} />
          FlowBoard
        </Link>

        <div className="flex-1" />

        <div className="flex items-center gap-3">
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 rounded-full bg-blue-100 text-blue-700 font-semibold text-sm flex items-center justify-center select-none">
              {initials}
            </div>
            <span className="text-sm text-gray-700 font-medium hidden sm:block">
              {user?.name}
            </span>
          </div>

          <button
            onClick={handleLogout}
            className="flex items-center gap-1.5 text-sm text-gray-500 hover:text-red-600 transition-colors px-2 py-1 rounded-lg hover:bg-red-50"
            title="Sair"
          >
            <LogOut size={16} />
            <span className="hidden sm:block">Sair</span>
          </button>
        </div>
      </header>

      <main className="flex-1">
        <Outlet />
      </main>
    </div>
  )
}
