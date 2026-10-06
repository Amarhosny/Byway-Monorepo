export default function Dashboard() {
  return (
    <>
      <div className="topbar"><h1>Dashboard</h1><div className="bell" /></div>
      <hr className="rule" />
      <div className="stat-row">
        <div className="stat-card"><div><div className="num">50</div><div className="lbl">Instructors</div></div><div className="stat-ic">👤</div></div>
        <div className="stat-card"><div><div className="num">10</div><div className="lbl">Categories</div></div><div className="stat-ic">📄</div></div>
        <div className="stat-card"><div><div className="num">36</div><div className="lbl">Courses</div></div><div className="stat-ic">📁</div></div>
      </div>
      <div className="panel-row">
        <div className="panel">
          <div style={{ display: 'flex', justifyContent: 'space-between' }}>
            <h3 style={{ margin: 0 }}>Wallet</h3>
            <span className="btn btn-ghost" style={{ padding: '6px 12px' }}>This month</span>
          </div>
          <div className="wallet-num">$37.5K</div>
          <div style={{ color: 'var(--ink-soft)', fontSize: 13.5 }}>
            Wallet Balance <span style={{ color: 'var(--green)' }}>▲ +2.45%</span>
          </div>
          <div className="pill-ok">✅ On your account</div>
          <div className="chart-wave" />
          <div className="legend">
            <span><span className="sw" style={{ background: 'var(--purple)' }} />Deposits</span>
            <span><span className="sw" style={{ background: '#7fd1ff' }} />Withdrawals</span>
          </div>
        </div>
        <div className="panel">
          <h3 style={{ margin: 0 }}>Statistics</h3>
          <div className="pie" />
          <div className="stat-legend">
            <div><span className="sw" style={{ background: 'var(--purple)' }} />Instructors<b>60%</b></div>
            <div><span className="sw" style={{ background: '#7fd1ff' }} />Categories<b>20%</b></div>
            <div><span className="sw" style={{ background: '#d9def7' }} />Courses<b>20%</b></div>
          </div>
        </div>
      </div>
    </>
  );
}
